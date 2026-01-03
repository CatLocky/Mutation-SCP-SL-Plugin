using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace Mutation
{
    public class Config : IConfig
    {
        [Description("Is plugin enabled or not")]
        public bool IsEnabled { get; set; } = true;
        [Description("Debug mode")]
        public bool Debug { get; set; } = false;
        [Description("Max mutations per round")]
        public int MaxMutations { get; set; } = 5;
        [Description("Mutation chance")]
        public int MutationChance { get; set; } = 50;
    }
}
