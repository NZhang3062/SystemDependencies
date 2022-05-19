using System;
using System.Collections.Generic;

namespace SystemDependencies
{
    internal class Software
    {
        private readonly String name;
        private readonly List<Software> dependencies;
        public Software(String name)
        {
            this.name = name;
            dependencies = new List<Software>();
        }
        public String GetName()
        {
            return name;
        }
        public List<Software> GetDependencies()
        {
            return dependencies;
        }
        public void AddDependencies(Software dependency)
        {
            this.dependencies.Add(dependency);
        }
    }
}