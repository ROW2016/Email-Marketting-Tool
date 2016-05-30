using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMT_WebApp.Models;

namespace EMT_WebApp.ViewModels
{
    public class ExceptionViewModel
    {
        public List<M_CustomException> exceptions { get; set; }

        public ExceptionViewModel()
        {
            exceptions = new List<M_CustomException>();
        }
    }
}