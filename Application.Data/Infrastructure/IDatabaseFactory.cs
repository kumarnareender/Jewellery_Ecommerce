using System;
using Application.Data.Models;

namespace Application.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        ApplicationEntities Get();
    }
}
