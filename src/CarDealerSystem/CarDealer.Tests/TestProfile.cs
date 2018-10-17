namespace CarDealer.Tests
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Common.AutoMapping.Interfaces;

    public class TestProfile : Profile
    {
        public TestProfile()
        {
            this.ConfigureProfile();
        }

        private void ConfigureProfile()
        {
            var allTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.GetName().FullName.Contains(nameof(CarDealer)))
                .SelectMany(a => a.GetTypes());

            var allMappingTypes = allTypes
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.GetInterfaces()
                                .Where(i => i.IsGenericType)
                                .Select(i => i.GetGenericTypeDefinition())
                                .Contains(typeof(IMapWith<>)))
                .Select(t => new
                {
                    Destination = t,
                    Sourse = t.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => new
                        {
                            Definition = i.GetGenericTypeDefinition(),
                            Arguments = i.GetGenericArguments()
                        })
                        .Where(i => i.Definition == typeof(IMapWith<>))
                        .SelectMany(i => i.Arguments)
                        .First()
                })
                .ToList();

            //Creates bidirectional mapping for all types, which extends IMapWith<TModel> interface
            foreach (var type in allMappingTypes)
            {
                this.CreateMap(type.Destination, type.Sourse);
                this.CreateMap(type.Sourse, type.Destination);
            }
        }
    }
}
