using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace dotnet_core_openid_client_credentials_grant_exercise
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // 將身份驗證服務添加到DI
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {

                // 發行權杖之驗證伺服器的位址。 JWT 持有人驗證中介軟體使用此 URI 來取得公開金鑰，以用來驗證權杖的簽章。 中介軟體也會確認權杖中的 iss 參數符合此 URI。
                options.Authority = Configuration["auth:oidc:AuthBaseUri"];


                // name of the API resource
                options.Audience = Configuration["auth:oidc:ApiName"];

#if DEBUG
                // 接受 Authority 使用非Https
                options.RequireHttpsMetadata = false;
#endif
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 將身份驗證中間件添加到管道中，以便對主機的每次調用都將自動執行身份驗證
            app.UseAuthentication();
            // 添加了授權中間件，以確保匿名客戶端無法訪問我們的API端點
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
