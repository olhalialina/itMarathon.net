﻿using ItMarathon.Dal.Common.Contracts;
using ItMarathon.Dal.Context;
using ItMarathon.Dal.Entities;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace ItMarathon.Dal.Repositories;

public class ProposalRepository(ApplicationDbContext repositoryContext) :
    RepositoryBase<Proposal>(repositoryContext), IProposalRepository
{
    public async Task<IEnumerable<Proposal>> GetProposalsAsync(bool trackChanges, ODataQueryOptions queryOptions)
    {
        IQueryable<Proposal> query = FindAll(trackChanges);

        if (queryOptions != null) 
        {
            query = (IQueryable<Proposal>)queryOptions.ApplyTo(query);
        }

        query = query
            .Include(p => p.AppUser)
            .Include(p => p.Photos)
            .Include(p => p.Properties!)
                .ThenInclude(properties => properties.PropertyDefinition)
            .Include(p => p.Properties!)
                .ThenInclude(properties => properties.PredefinedValue)
                    .ThenInclude(prop => prop!.ParentPropertyValue);

        return await query.ToListAsync();
    }

    public async Task<Proposal?> GetProposalAsync(long proposalId, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(proposalId), trackChanges)
        .Include(p => p.AppUser)
        .Include(p => p.Photos)
        .Include(p => p.Properties!)
            .ThenInclude(properties => properties.PropertyDefinition)
        .Include(p => p.Properties!)
            .ThenInclude(properties => properties.PredefinedValue)
                .ThenInclude(prop => prop!.ParentPropertyValue)
        .SingleOrDefaultAsync();

    public void CreateProposal(Proposal proposal) => Create(proposal);

    public void DeleteProposal(Proposal proposal) => Delete(proposal);
}
