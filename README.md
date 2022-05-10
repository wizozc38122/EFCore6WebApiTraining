# EF Core 6 Repository

## 說明

個人練習, .NET 6 WebApi Repository Pattern

## 相關指令

```ps
# EF Core 6
dotnet add package Microsoft.EntityFrameworkCore

## 資料庫連接套件,依照需要的資料庫安裝
# .net cli
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
# nuget
Install-Package Microsoft.EntityFrameworkCore.SqlServer


## 遷移工具
# .net cli 
dotnet tool install --global dotnet-ef

## 遷移套件 
# .net cli
dotnet add package Microsoft.EntityFrameworkCore.Design
# nuget
Install-Package Microsoft.EntityFrameworkCore.Tools

## 資料庫遷移
# 建立遷移檔
dotnet ef migrations add init
# 更新/建立資料庫
dotnet ef database update
```

## 參考

[官方-範例模型](https://docs.microsoft.com/zh-tw/ef/core/get-started/overview/first-app?tabs=netcore-cli)

[官方-Fluent API](https://docs.microsoft.com/zh-tw/ef/ef6/modeling/code-first/fluent/relationships)

[Will - 認識 Entity Framework Core 載入關聯資料的三種方法](https://blog.miniasp.com/post/2022/04/21/Loading-Related-Data-in-EF-Core)

# 範例實作

## - 基本

依照官方基礎範例模型練習

1. 建立資料庫物件/DbContext, 並建立資料庫

    建立Repository目錄存放

    ```ps
    # 安裝套件
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    dotnet tool install --global dotnet-ef
    dotnet add package Microsoft.EntityFrameworkCore.Design 
    ```
    
    1. 建立Entites目錄 - 資料庫物件
        ```C#
        // Repository/Entites/Blog.cs

        namespace EFCore6WebApiTraining.Repository.Entities
        {
            public class Blog
            {
                public int BlogId { get; set; }
                public string? Url { get; set; }

                public List<Post> Posts { get; } = new();
            }
        }

        // Repository/Entites/Post.cs

        namespace EFCore6WebApiTraining.Repository.Entities
        {
            public class Post
            {
                public int PostId { get; set; }
                public string? Title { get; set; }
                public string? Content { get; set; }

                public int BlogId { get; set; }
                public Blog? Blog { get; set; }
            }
        }
        ```

    2. 建立Db目錄 - 設定資料庫欄位&關聯

        Fluent API

        ```C#
        // Repository/Db/BlogEntityTypeConfiguration.cs

        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Entities;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;

        namespace EFCore6WebApiTraining.Repository.Db
        {
            public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
            {
                public void Configure(EntityTypeBuilder<Blog> builder)
                {
                    builder.ToTable("Blog");
                    builder.HasKey(blog => blog.BlogId);
                    builder.Property(blog => blog.Url).HasMaxLength(250).HasColumnName("Url").IsRequired();

                    // 外鍵 & 必須有Blog才有Post, 因此同時刪除
                    builder.HasMany(blog => blog.Posts)
                        .WithOne(post => post.Blog)
                        .HasForeignKey(post => post.BlogId)
                        .OnDelete(DeleteBehavior.Cascade);
                }
            }
        }

        // Repository/Db/PostEntityTypeConfiguration.cs

        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Entities;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;

        namespace EFCore6WebApiTraining.Repository.Db
        {
            public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
            {
                public void Configure(EntityTypeBuilder<Post> builder)
                {
                    builder.ToTable("Post");
                    builder.HasKey(post => post.PostId);
                    builder.Property(post => post.BlogId).HasColumnName("BlogId").IsRequired();
                    builder.Property(post => post.Title).HasMaxLength(50).HasColumnName("Title").IsRequired();
                    builder.Property(post => post.Content).HasMaxLength(250).HasColumnName("Content").IsRequired();
                }
            }
        }
        ```

    3. DbContext
        ```C#
        // Repository/Db/BloggingContext.cs

        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Entities;

        namespace EFCore6WebApiTraining.Repository.Db
        {
            
            public class BloggingContext : DbContext
            {
                public DbSet<Blog>? Blogs { get; set; }
                public DbSet<Post>? Posts { get; set; }

                public BloggingContext(DbContextOptions<BloggingContext> options) : base(options) 
                {
                }
                
                // 覆寫, 套用
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.ApplyConfiguration(new BlogEntityTypeConfiguration());
                    modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());

                    base.OnModelCreating(modelBuilder);
                }

            }

            
        }
        ```
    
    4. 新增資料庫連接字串 - Sqlite
        ```json
        // appsettings.json

        "ConnectionStrings": {
            "Sqlite": "Data Source=./EFCore6WebApiTraining.db",
            "SqlServer": "Server=localhost\\SQLEXPRESS;Database=EFCore6WebApiTraining;Trusted_Connection=True;MultipleActiveResultSets=True;User ID='';Password=''"
        }
        ```
    
    5. DbContext 依賴注入
        ```C#
        // Program.cs

        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Db;

        builder.Services.AddDbContext<BloggingContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

        builder.Services.AddDbContext<BloggingContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
        ```
    
    5. 建立遷移檔&建立資料庫
        ```ps
        # 建立遷移檔
        dotnet ef migrations add init

        # 更新資料庫
        dotnet ef database update
        ```
2. 基本CRUD操作

    1. 建立Interfaces目錄 - 建立Blog CRUD介面

        ```C#
        // Repository/Interfaces/IBlogRepository.cs

        using EFCore6WebApiTraining.Repository.Entities;

        namespace EFCore6WebApiTraining.Repository.Interfaces
        {
            public interface IBlogRepository
            {
                // C
                Task<bool> CreateAsync(Blog blog);

                // R
                Task<Blog?> GetByIdAsync(int Id);
                Task<IEnumerable<Blog>> GetAllAsync();

                // U
                Task<bool> UpdateAsync(Blog blog);

                // Delete
                Task<bool> DeleteByIdAsync(int id);

            }
        }
        ```
    
    2. 建立Implements目錄 - 建立Blog CRUD實作

        ```C#
        // Repository/Implements/BlogRepository.cs

        using EFCore6WebApiTraining.Repository.Entities;
        using EFCore6WebApiTraining.Repository.Interfaces;
        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Db;

        namespace EFCore6WebApiTraining.Repository.Implements
        {
            public class BlogRepository : IBlogRepository
            {
                private readonly BloggingContext _context;

                public BlogRepository(BloggingContext context)
                {
                    _context = context;
                }
                
                // C
                public async Task<bool> CreateAsync(Blog blog)
                {
                    _context.Set<Blog>().Add(blog);
                    var count = await _context.SaveChangesAsync();

                    return count > 0;
                }

                // R
                public async Task<Blog?> GetByIdAsync(int id)
                {
                    return await _context.Set<Blog>().FindAsync(id);
                }

                public async Task<IEnumerable<Blog>> GetAllAsync()
                {
                    return await _context.Set<Blog>().ToListAsync();
                }

                // U
                public async Task<bool> UpdateAsync(Blog blog)
                {
                    _context.Set<Blog>().Update(blog);
                    var count = await _context.SaveChangesAsync();

                    return count > 0;
                }

                // D
                public async Task<bool> DeleteByIdAsync(int id)
                {
                    var target = await GetByIdAsync(id);
                    if (target == null) { return false; }

                    _context.Set<Blog>().Remove(target);

                    var count = await _context.SaveChangesAsync();

                    return count > 0;
                }

            }
        }
        ```

    3. 注入介面實作

        ```C#
        // Program.cs
        using Microsoft.EntityFrameworkCore;
        using EFCore6WebApiTraining.Repository.Interfaces;
        using EFCore6WebApiTraining.Repository.Implements;


        builder.Services.AddTransient<IBlogRepository, BlogRepository>();
        ```

    4. Parameters目錄 - 前端輸入參數

        由於id自動生成所以不需要BlogId
        ```C#
        // Parameters/BlogCreateParameter.cs
        
        using System.ComponentModel.DataAnnotations;
        
        namespace EFCore6WebApiTraining.Parameter
        {
            public class BlogCreateParameter
            {
                // Url 必填限制
                [Required]
                public string? Url { get; set; }
            }
        }
        ```

    5. 建立 BlogController

        撰寫簡易CRUD API

        ```C#
        // Controllers/BlogController.cs

        using Microsoft.AspNetCore.Mvc;
        using EFCore6WebApiTraining.Repository.Interfaces;
        using EFCore6WebApiTraining.Repository.Entities;
        using EFCore6WebApiTraining.Parameter;


        namespace EFCore6WebApiTraining.Controllers
        {
            [Route("api/[controller]")]
            [ApiController]
            public class BlogController : ControllerBase
            {
                private readonly IBlogRepository _blogRepository;

                public BlogController(IBlogRepository blogRepository) => _blogRepository = blogRepository;

                // C
                [HttpPost]
                public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
                {
                    var result = await _blogRepository.CreateAsync(new Blog() { Url=blog.Url });

                    return Ok(result);
                }

                // R
                [HttpGet("{id}")]
                public async Task<IActionResult> GetByIdAsync(int id)
                {
                    var result = await _blogRepository.GetByIdAsync(id);

                    if (result == null)
                    {
                        return NotFound();
                    }

                    return Ok(result);
                }
                [HttpGet]
                public async Task<IActionResult> GetAllAsync()
                {
                    var result = await _blogRepository.GetAllAsync();

                    if (result == null)
                    {
                        return NotFound();
                    }

                    return Ok(result);
                }

                // U
                [HttpPut]
                public async Task<IActionResult> UpdateAsync(Blog blog)
                {
                    var result = await _blogRepository.UpdateAsync(blog);

                    return Ok(result);
                }


                // D
                [HttpDelete("{id}")]
                public async Task<IActionResult> DeleteByIdAsync(int id)
                {
                    var result = await _blogRepository.DeleteByIdAsync(id);

                    return Ok(result);
                }

            }
        }
        ```

## - 泛型 Repository

接下來Post也要同樣做一次CRUD, 都是相同操作

都依賴同一個DbContext, 只差在Entity, 因此可以做泛型介面實作

1. 建立泛型介面

    ```C#
    // Repository/Interfaces/IGenericRepository.cs

    namespace EFCore6WebApiTraining.Repository.Interfaces
    {
        public interface IGenericRepository<TEntity> where TEntity : class
        {
            // C
            Task<bool> CreateAsync(TEntity entity);

            // R
            Task<TEntity?> GetByIdAsync(int Id);

            Task<IEnumerable<TEntity>> GetAllAsync();

            // U
            Task<bool> UpdateAsync(TEntity entity);

            // Delete
            Task<bool> DeleteByIdAsync(int id);
        }
    }

    ```
2. 建立泛型實作

    ```C#
    // Repository/Implements/GenericRepository.cs

    using EFCore6WebApiTraining.Repository.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using EFCore6WebApiTraining.Repository.Db;

    namespace EFCore6WebApiTraining.Repository.Implements
    {
        public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
        {
            // 修改繼承自DbContext
            private readonly BloggingContext _context;

            public GenericRepository(BloggingContext context)
            {
                _context = context;
            }
            
            // C
            public async Task<bool> CreateAsync(TEntity entity)
            {
                _context.Set<TEntity>().Add(entity);
                var count = await _context.SaveChangesAsync();

                return count > 0;
            }

            // R
            public async Task<TEntity?> GetByIdAsync(int id)
            {
                return await _context.Set<TEntity>().FindAsync(id);
            }

            public async Task<IEnumerable<TEntity>> GetAllAsync()
            {
                return await _context.Set<TEntity>().ToListAsync();
            }

            // U
            public async Task<bool> UpdateAsync(TEntity entity)
            {
                _context.Set<TEntity>().Update(entity);
                var count = await _context.SaveChangesAsync();

                return count > 0;
            }

            // D
            public async Task<bool> DeleteByIdAsync(int id)
            {
                var target = await GetByIdAsync(id);
                if (target == null) { return false; }

                _context.Set<TEntity>().Remove(target);

                var count = await _context.SaveChangesAsync();

                return count > 0;
            }

        }
    }
    ```

3. 修改實作注入

    ```C#
    // Program.cs
    
    using Microsoft.EntityFrameworkCore;
    using EFCore6WebApiTraining.Repository.Interfaces;
    using EFCore6WebApiTraining.Repository.Implements;
    using EFCore6WebApiTraining.Repository.Entities;


    builder.Services.AddTransient<IGenericRepository<Blog>, GenericRepository<Blog>>();
    builder.Services.AddTransient<IGenericRepository<Post>, GenericRepository<Post>>();
    ```

4. 修改BlogCrontroller

    ```C#
    // Controllers/BlogController.cs

    // 只需要修改泛型介面實作的部分
    // _blogRepository欄位命名不變
    private readonly IGenericRepository<Blog> _blogRepository;

    public BlogGenericRepositoryController(IGenericRepository<Blog> blogRepository) => _blogRepository = blogRepository;
    ```

5. 同理Post相同方式實作即可

    ```C# 
    // Parameters/PostCreateParameter.cs
    using System.ComponentModel.DataAnnotations;

    // PostId自動, 必須要有Blog才有Post必須知道外鍵所屬
    namespace EFCore6WebApiTraining.Parameters
    {
        public class PostCreateParameter
        {
            [Required]
            public string? Title { get; set; }
            [Required]
            public string? Content { get; set; }
            [Required]
            public int BlogId { get; set; }
        }
    }

    // Parameters/PostUpdateParameter.cs
    using System.ComponentModel.DataAnnotations;

    // Entity有Blog欄位, 修改Post不該出現
    namespace EFCore6WebApiTraining.Parameters
    {
        public class PostUpdateParameter
        {
            [Required]
            public int PostId { get; set; }

            [Required]
            public string? Title { get; set; }
            [Required]
            public string? Content { get; set; }
        }
    }


    // Controllers/PostController.cs

    
    using Microsoft.AspNetCore.Mvc;
    using EFCore6WebApiTraining.Repository.Interfaces;
    using EFCore6WebApiTraining.Repository.Entities;
    using EFCore6WebApiTraining.Parameters;


    namespace EFCore6WebApiTraining.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PostGenericRepositoryController : ControllerBase
        {
            // Create 關係需要用到BlogRepo
            private readonly IGenericRepository<Blog> _blogRepository;
            private readonly IGenericRepository<Post> _postRepository;

            public PostGenericRepositoryController(IGenericRepository<Blog> blogRepository, IGenericRepository<Post> postRepository)
            {
                _blogRepository = blogRepository;
                _postRepository = postRepository;
            }

            // C
            // 有Blog才有Post, 因此要先確認存在
            [HttpPost]
            public async Task<IActionResult> CreateAsync(PostCreateParameter post)
            {
                var blog = _blogRepository.GetByIdAsync(post.BlogId);

                if (blog == null) { return BadRequest(); }

                var result = await _postRepository.CreateAsync(new Post()
                {
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                });

                return Ok(result);
            }

            // R
            [HttpGet("{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var result = await _postRepository.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            [HttpGet]
            public async Task<IActionResult> GetAllAsync()
            {
                var result = await _postRepository.GetAllAsync();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            // U
            // 先取出舊資料, 確認存在修改舊資料
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(PostUpdateParameter post)
            {
                var target = await _postRepository.GetByIdAsync(post.PostId);

                if(target == null) { return BadRequest(); }

                target.Title = post.Title;
                target.Content = post.Content;

                var result = await _postRepository.UpdateAsync(target);

                return Ok(result);
            }


            // D
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteByIdAsync(int id)
            {
                var result = await _postRepository.DeleteByIdAsync(id);

                return Ok(result);
            }
        }
    }

    ```


## - 泛型 Unit of Work 工作單元

DbContext = 資料庫, DbSet = 資料表

源自於同一個資料庫的操作可以一起進行 

不必每次操作都要進行一次連線

複數操作也能最後一起 Commit

也能將方法統一在一個實作中, 不用多個Repository

1. 修改並建立新的 GenericRepository

    由於最後才Commit所以將原本Add/Update/Delete的Commit動作移除

    建立 Repository/UnitOfWork 目錄存放

    ```C#
    // Repository/UnitOfWork/Interfaces/IUnitOfWorkGenericRepository.cs

    // 介面, 由於操作都抽離, 所以僅剩下void
    namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
    {
        public interface IUnitOfWorkGenericRepository<TEntity> where TEntity : class
        {
            // C
            void Create(TEntity entity);

            // R
            Task<TEntity?> GetByIdAsync(int Id);

            Task<IEnumerable<TEntity>> GetAllAsync();

            // U
            void Update(TEntity entity);

            // Delete
            void Delete(TEntity entity);
        }
    }

    // Repository/UnitOfWork/Implements/UnitOfWorkGenericRepository.cs

    // 實作
    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Db;
    using Microsoft.EntityFrameworkCore;

    namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
    {
        public class UnitOfWorkGenericRepository<TEntity> : IUnitOfWorkGenericRepository<TEntity> where TEntity : class
        {
            private readonly BloggingContext _context;
            private readonly DbSet<TEntity> _dbSet;

            public UnitOfWorkGenericRepository(BloggingContext context)
            {
                _context = context;
                _dbSet = _context.Set<TEntity>();
            }

            // C
            public void Create(TEntity entity)
            {
                _dbSet.Add(entity);
            }

            // R
            public async Task<TEntity?> GetByIdAsync(int id)
            {
                return await _dbSet.FindAsync(id);
            }

            public async Task<IEnumerable<TEntity>> GetAllAsync()
            {
                return await _dbSet.ToListAsync();
            }

            // U
            public void Update(TEntity entity)
            {
                _dbSet.Update(entity);
            }

            // D
            public void Delete(TEntity entity)
            {
                _dbSet.Remove(entity);

            }
        }
    }
    ```

2. UnitOfWork實作

    ```C#
    // Repository/UnitOfWork/Interfaces/IGenericUnitOfWork.cs

    // 介面
    namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
    {
        // 繼承IDisposable用來關閉連線, 回收資源
        public interface IGenericUnitOfWork : IDisposable 
        {
            // 選擇Repo
            IUnitOfWorkGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

            // 將SaveChange抽離
            Task<int> SaveChangeAsync();
        }
    }

    // Repository/UnitOfWork/Implements/GenericUnitOfWork.cs

    // 實作
    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Db;
    using System.Collections;

    namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
    {
        public class GenericUnitOfWork : IGenericUnitOfWork
        {
            private bool disposedValue = false;
            private readonly BloggingContext _context;
            private Hashtable? _repositories;


            public GenericUnitOfWork(BloggingContext context)
            {
                _context = context;
            }

            public IUnitOfWorkGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
            {
                if (_repositories == null)
                {
                    _repositories = new Hashtable();
                }

                var type = typeof(TEntity);

                if (!_repositories.ContainsKey(type))
                {
                    _repositories[type] = new UnitOfWorkGenericRepository<TEntity>(_context);
                }

                return (IUnitOfWorkGenericRepository<TEntity>)_repositories[type];
            }

            public async Task<int> SaveChangeAsync()
            {
                return await _context.SaveChangesAsync();
            }


            // IDisposable

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _context.Dispose();
                    }

                    disposedValue = true;
                }
            }
            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }

    ```

3.  實作注入

    ```C#
    // Program.cs

    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.UnitOfWork.Implements;

    builder.Services.AddScoped<IGenericUnitOfWork, GenericUnitOfWork>();
    ```

4. Controller

    ```C#
    // Controllers/BlogGenericUnitOfWorkController.cs

    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Entities;
    using EFCore6WebApiTraining.Parameters;
    using Microsoft.AspNetCore.Mvc;

    namespace EFCore6WebApiTraining.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class BlogGenericUnitOfWorkController : ControllerBase
        {
            private readonly IGenericUnitOfWork _unitOfWork;

            public BlogGenericUnitOfWorkController(IGenericUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

            // C
            [HttpPost]
            public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
            {
                _unitOfWork.GetRepository<Blog>().Create(new Blog() { Url = blog.Url });

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }

            // R
            [HttpGet("{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var result = await _unitOfWork.GetRepository<Blog>().GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            [HttpGet]
            public async Task<IActionResult> GetAllAsync()
            {
                var result = await _unitOfWork.GetRepository<Blog>().GetAllAsync();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            // U
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(Blog blog)
            {
                _unitOfWork.GetRepository<Blog>().Update(blog);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }


            // D
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteByIdAsync(int id)
            {
                var blogRepository = _unitOfWork.GetRepository<Blog>();

                var blog = await blogRepository.GetByIdAsync(id);

                if (blog == null) { return BadRequest(); }

                blogRepository.Delete(blog);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }
        }
    }

    // Controllers/PostGenericUnitOfWorkController.cs

    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Entities;
    using EFCore6WebApiTraining.Parameters;
    using Microsoft.AspNetCore.Mvc;

    namespace EFCore6WebApiTraining.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PostGenericUnitOfWorkController : ControllerBase
        {
            private readonly IGenericUnitOfWork _unitOfWork;

            public PostGenericUnitOfWorkController(IGenericUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
            // C
            [HttpPost]
            public async Task<IActionResult> CreateAsync(PostCreateParameter post)
            {
                var blog = _unitOfWork.GetRepository<Blog>().GetByIdAsync(post.BlogId);

                if (blog == null) { return BadRequest(); }

                _unitOfWork.GetRepository<Post>().Create(new Post()
                {
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                });

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }

            // R
            [HttpGet("{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var result = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            [HttpGet]
            public async Task<IActionResult> GetAllAsync()
            {
                var result = await _unitOfWork.GetRepository<Post>().GetAllAsync();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

            // U
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(PostUpdateParameter post)
            {
                var target = await _unitOfWork.GetRepository<Post>().GetByIdAsync(post.PostId);

                if (target == null) { return BadRequest(); }

                target.Title = post.Title;
                target.Content = post.Content;

                _unitOfWork.GetRepository<Post>().Update(target);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }


            // D
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteByIdAsync(int id)
            {
                var postRepository = _unitOfWork.GetRepository<Post>();


                var post = await postRepository.GetByIdAsync(id);

                if(post == null) { return BadRequest(); }

                postRepository.Delete(post);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }
        }
    }
    ```

## - Unit Of Work 不同功能的 Repository

使用泛型可以很方便依照Entity變更而不用新增實作方法, 

但有時候有些兩張表個需求不同, 就要分開來寫Repositry實作

但還是同一個資料庫, 因此依然可以用UOW方式

1. 實作Blog/Post

    1. 繼承基底介面
    
        繼承IUnitOfWorkGenericRepository當作基本要實作的方法
            
        兩者都有基礎方法, 但實作不同, 或者後續衍生不同再補上個別interface中

        ```c#
        // Repository/UnitOfWork/Interfaces/IUnitOfWorkBlogRepository.cs

        namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
        {
            public interface IUnitOfWorkBlogRepository : IUnitOfWorkGenericRepository<Blog>
            {
            }
        }

        // Repository/UnitOfWork/Interfaces/IUnitOfWorkPostRepository.cs

        namespace EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces
        {
            public interface IUnitOfWorkBlogRepository : IUnitOfWorkGenericRepository<Post>
            {
            }
        }
        ```
    
    2. 實作

        EF Core後就不會預先載入了
        
        假設取出Blog時要包含Post, 修改Get/GetAll

        因此這個改變與Post的泛型就衝突了, Post沒有需要包含Blog

        ```C#
        // Repository/UnitOfWork/Implements/UnitOfWorkBlogRepository.cs

        using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
        using EFCore6WebApiTraining.Repository.Db;
        using EFCore6WebApiTraining.Repository.Entities;
        using Microsoft.EntityFrameworkCore;

        namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
        {
            public class UnitOfWorkBlogRepository : IUnitOfWorkBlogRepository
            {
                private readonly BloggingContext _context;
                private readonly DbSet<Blog> _dbSet;

                public UnitOfWorkBlogRepository(BloggingContext context)
                {
                    _context = context;
                    _dbSet = _context.Set<Blog>();
                }

                // C
                public void Create(Blog blog)
                {
                    _dbSet.Add(blog);
                }

                // R
                // Include包含
                public async Task<Blog?> GetByIdAsync(int id)
                {
                    return await _dbSet.Include(blog => blog.Posts).SingleOrDefaultAsync();
                }

                public async Task<IEnumerable<Blog>> GetAllAsync()
                {
                    return await _dbSet.Include(blog => blog.Posts).ToListAsync();
                }

                // U
                public void Update(Blog blog)
                {
                    _dbSet.Update(blog);
                }

                // D
                public void Delete(Blog blog)
                {
                    _dbSet.Remove(blog);

                }
            }
        }
        ```

        Post 基本沒變
        ```C#
        // Repository/UnitOfWork/Implements/UnitOfWorkPostRepository.cs

        using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
        using EFCore6WebApiTraining.Repository.Db;
        using EFCore6WebApiTraining.Repository.Entities;
        using Microsoft.EntityFrameworkCore;

        namespace EFCore6WebApiTraining.Repository.UnitOfWork.Implements
        {
            public class UnitOfWorkPostRepository : IUnitOfWorkPostRepository
            {
                private readonly BloggingContext _context;
                private readonly DbSet<Post> _dbSet;

                public UnitOfWorkPostRepository(BloggingContext context)
                {
                    _context = context;
                    _dbSet = _context.Set<Post>();
                }

                // C
                public void Create(Post post)
                {
                    _dbSet.Add(post);
                }

                // R
                public async Task<Post?> GetByIdAsync(int id)
                {
                    return await _dbSet.FindAsync(id);
                }

                public async Task<IEnumerable<Post>> GetAllAsync()
                {
                    return await _dbSet.ToListAsync();
                }

                // U
                public void Update(Post post)
                {
                    _dbSet.Update(post);
                }

                // D
                public void Delete(Post post)
                {
                    _dbSet.Remove(post);

                }
            }
        }
        ```
2. 建立ViewModel目錄

    也避免直接取得Entity時包含對方欄位, 會導致迴圈

    Post 不需要 Blog實體
    ```C#
    // ViewModel/PostViewModel.cs

    namespace EFCore6WebApiTraining.ViewModel
    {
        public class PostViewModel
        {
            public int PostId { get; set; }
            public string? Title { get; set; }
            public string? Content { get; set; }

            public int BlogId { get; set; }
        }
    }
    ```

    Blog 基本相同, 但避免迴圈就抽出來,
    
    也不希望補`[JsonIgnore]`屬性, 去更動到Entity, 保持乾淨
    ```C#
    // ViewModel/BlogViewModel.cs

    namespace EFCore6WebApiTraining.ViewModel
    {
        public class BlogViewModel
        {
            public int BlogId { get; set; }

            public string? Url { get; set; }

            public List<PostViewModel>? Posts { get; set; }
        }
    }
    ```

3. 實作注入
    ```C#
    // Program.cs

    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.UnitOfWork.Implements;

    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
    ```

4. Controller
    ```C#
    // Controllers/BlogUnitOfWorkController.cs

    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Entities;
    using EFCore6WebApiTraining.Parameters;
    using EFCore6WebApiTraining.ViewModel;
    using Microsoft.AspNetCore.Mvc;

    namespace EFCore6WebApiTraining.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class BlogUnitOfWorkController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;

            public BlogUnitOfWorkController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

            // C
            [HttpPost]
            public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
            {

                _unitOfWork.Blogs.Create(new Blog() { Url = blog.Url });

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }

            // R
            [HttpGet("{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var result = await _unitOfWork.Blogs.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }



                return Ok(new BlogViewModel()
                {
                    BlogId = result.BlogId,
                    Url = result.Url,
                    Posts = result.Posts?.Select(post => new PostViewModel()
                    {
                        PostId = post.PostId,
                        Title = post.Title,
                        Content = post.Content,
                        BlogId = post.BlogId,
                    }).ToList()
                });
            }
            [HttpGet]
            public async Task<IActionResult> GetAllAsync()
            {
                var result = await _unitOfWork.Blogs.GetAllAsync();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result.Select(blog => new BlogViewModel()
                {
                    BlogId = blog.BlogId,
                    Url = blog.Url,
                    Posts = blog.Posts?.Select(post => new PostViewModel()
                    {
                        PostId = post.PostId,
                        Title = post.Title,
                        Content = post.Content,
                        BlogId = post.BlogId,
                    }).ToList(),
                }));
            }

            // U
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(Blog blog)
            {
                _unitOfWork.Blogs.Update(blog);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }


            // D
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteByIdAsync(int id)
            {
                var blog = await _unitOfWork.Blogs.GetByIdAsync(id);

                if (blog == null) { return BadRequest(); }

                _unitOfWork.Blogs.Delete(blog);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }
        }
    }

    ```
    ```C#
    // Controllers/PostUnitOfWorkController.cs
    using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
    using EFCore6WebApiTraining.Repository.Entities;
    using EFCore6WebApiTraining.Parameters;
    using EFCore6WebApiTraining.ViewModel;
    using Microsoft.AspNetCore.Mvc;

    namespace EFCore6WebApiTraining.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PostUnitOfWorkController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;

            public PostUnitOfWorkController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
            // C
            [HttpPost]
            public async Task<IActionResult> CreateAsync(PostCreateParameter post)
            {
                var blog = _unitOfWork.Blogs.GetByIdAsync(post.BlogId);

                if (blog == null) { return BadRequest(); }

                _unitOfWork.Posts.Create(new Post()
                {
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                });

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }

            // R
            [HttpGet("{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var result = await _unitOfWork.Posts.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok( new PostViewModel() { 
                    PostId = result.PostId,
                    Title = result.Title,
                    Content= result.Content,
                    BlogId= result.BlogId,
                });
            }

            [HttpGet]
            public async Task<IActionResult> GetAllAsync()
            {
                var result = await _unitOfWork.Posts.GetAllAsync();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result.Select(post => new PostViewModel()
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                }));

            }

            // U
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(PostUpdateParameter post)
            {
                var target = await _unitOfWork.Posts.GetByIdAsync(post.PostId);

                if (target == null) { return BadRequest(); }

                target.Title = post.Title;
                target.Content = post.Content;

                _unitOfWork.Posts.Update(target);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }


            // D
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteByIdAsync(int id)
            {
                var postRepository = _unitOfWork.Posts;


                var post = await postRepository.GetByIdAsync(id);

                if (post == null) { return BadRequest(); }

                postRepository.Delete(post);

                var count = await _unitOfWork.SaveChangeAsync();

                return Ok(count > 0);
            }
        }
    }
    ```