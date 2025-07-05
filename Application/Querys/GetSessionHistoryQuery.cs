﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Querys
{
    public class GetSessionHistoryQuery : IRequest<SessionReconstructionDto>
    {
        public Guid SessionId { get; set; }
    }
}
