﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ISlideRepository
    {
        Task<string> GetCorrectAnswerAsync(int slideId);
    }
}
