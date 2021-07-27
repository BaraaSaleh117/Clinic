using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicProject.Models
{

    public class CountryModel
    {

        public CountryModel()
        {

        }

        public CountryModel(string Name)
        {
            Name = name;
        }

        public string name { get; set; }

    }
}