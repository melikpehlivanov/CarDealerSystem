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
<p>In order to run the project you just need to replace the connection string which is located in appsettings.json and run the project. There is a seed method which will insert all manufacturers, models, fuel types, transmission types and etc. Database seeding should take about 1 hour and 25 minutes depending on your machine.(Not recommended) Please keep in mind that you can reduce the time to 2-3 minutes by removing or commenting "SeedOptionalData" method in ApplicationBuilderExtensions.cs but you wont have any users and ads(vehicles) listed.</p>

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
    
<p>P.S Please keep in mind this project is in beta version and bugs are highly possible.</p>
