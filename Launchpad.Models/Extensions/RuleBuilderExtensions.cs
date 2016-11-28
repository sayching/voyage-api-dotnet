﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launchpad.Models.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithErrorCodeMessage<T,TProperty>(this IRuleBuilderOptions<T,TProperty> options, string code, string message)
        {
            options.WithMessage("{0}::{1}", code, message);
            return options;
        }
    }
}
