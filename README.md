# CarDealerSystem
<p>CarDealer is a system for buying/selling cars in which the users can register and login in order to add vehicles for sale. Vehicle details page shows pretty much all the possible information you would want to know about the vehicle. In the project are also implemented different user roles and areas separated in private sections for admins and regular users. Depending on its role every user can access different sections of the application and has specific permissions of what can or cannot do. P.S In the future many more things will be added.</p>
<p>Used technologies</p>
<ul>
  <li>Asp.Net Core 2.1</li>
  <li>Entity Framework Core</li>
</ul>

<p>Features</p>
<ul>
<li>Guest users can view vehicles profile page</li>
<li>Guest users can search vehicles</li>
<li>Guest users can register and login including Facebook and Google authentication</li>
<li>After successfull registration user must confirm his/her email, in order to be able to login</li>
<li>Registered users can place vehicles for sale</li>
<li>Registered users can create/update/delete their ads</li>
<li>Admin users can create/update/delete all vehicles, manufacturers and models</li>
<li>Admin users can see all users details including vehicles, roles, etc.</li>
<li>Admin users can add/remove users to roles</li>
<li>All Admin actions are logged</li>
<li>Only Senior Administrators can see log information about what admins have done.</li>
</ul>

<h3>Getting started:</h3>
<p>In order to run the project you just need to replace the connection string which is located in appsettings.json and run the project. There is a seed method which will insert all manufacturers, models, fuel types, transmission types and etc. Database seeding should take about 1 hour and 25 minutes depending on your machine. Please keep in mind that you can reduce the time to 2-3 minutes by removing or commenting "SeedOptionalData" method in ApplicationBuilderExtensions.cs but you wont have any users and ads(vehicles) listed and it's NOT recommended.</p>

<pre>public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<CarDealerDbContext>();

                dbContext.Database.Migrate();
                SeedRequiredData(dbContext);

                // Delete (Not recommended) or comment this method if you'd like to NOT have initial database with users and vehicles.
                SeedOptionalData(serviceScope, dbContext);
            }

            return app;
        }
    }</pre>
    
<p>The next thing which you MUST do if you want to have facebook and google login functionality is to create your own user secrets. You can see more details here:
<ul>
  <li><a href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-2.1&tabs=aspnetcore2x" target="_blank">Facebook</a></li>
  <li><a href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-2.1&tabs=aspnetcore2x" target="_blank">Google</a></li>
</ul>
And if you want to skip this part simply delete this block of code which is located in Startup.cs
<pre>services
                .AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppId").Value;
                    facebookOptions.AppSecret = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppSecret").Value;
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = this.Configuration.GetSection("ExternalAuthentications:Google:ClientId").Value;
                    googleOptions.ClientSecret = this.Configuration.GetSection("ExternalAuthentications:Google:ClientSecret").Value;
                });</pre>
<p> 
  The last and only thing which you need to do is put in your sendgrid Api key in your user secrets file - here's a <a href="https://sendgrid.com/docs/ui/account-and-settings/api-keys/#creating-an-api-key">guide</a>.
  
  Example of how your file should look like if you've implemented everything correctly.
  <pre>{
  "ExternalAuthentications": {
    "Facebook": {
      "AppId": "YourAppId",
      "AppSecret": "YourAppSecret"
    },
    "Google": {
      "ClientId": "YourClientId",
      "ClientSecret": "YourClientSecret"
    }
  },
  "EmailSetting": {
    "SendGridApiKey": "YourSendgridApiKey"
  }
}</pre>
