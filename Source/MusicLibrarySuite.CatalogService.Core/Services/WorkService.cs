using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MusicLibrarySuite.CatalogService.Core.Services.Abstractions;
using MusicLibrarySuite.CatalogService.Data.Entities;
using MusicLibrarySuite.CatalogService.Data.Entities.Base;
using MusicLibrarySuite.CatalogService.Data.Repositories.Abstractions;
using MusicLibrarySuite.CatalogService.Interfaces.Entities;

namespace MusicLibrarySuite.CatalogService.Core.Services;

/// <summary>
/// Represents a work service.
/// </summary>
public class WorkService : IWorkService
{
    private readonly IWorkRepository m_workRepository;
    private readonly IMapper m_mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkService" /> type using the specified services.
    /// </summary>
    /// <param name="workRepository">The work repository.</param>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public WorkService(IWorkRepository workRepository, IMapper mapper)
    {
        m_workRepository = workRepository;
        m_mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Work?> GetWorkAsync(Guid workId)
    {
        WorkDto? workDto = await m_workRepository.GetWorkAsync(workId);
        Work? work = m_mapper.Map<Work?>(workDto);
        return work;
    }

    /// <inheritdoc />
    public async Task<Work[]> GetWorksAsync()
    {
        WorkDto[] workDtoArray = await m_workRepository.GetWorksAsync();
        Work[] workArray = m_mapper.Map<Work[]>(workDtoArray);
        return workArray;
    }

    /// <inheritdoc />
    public async Task<Work[]> GetWorksAsync(IEnumerable<Guid> workIds)
    {
        WorkDto[] workDtoArray = await m_workRepository.GetWorksAsync(workIds);
        Work[] workArray = m_mapper.Map<Work[]>(workDtoArray);
        return workArray;
    }

    /// <inheritdoc />
    public async Task<WorkPageResponse> GetWorksAsync(WorkRequest workRequest)
    {
        WorkRequestDto workRequestDto = m_mapper.Map<WorkRequestDto>(workRequest);
        PageResponseDto<WorkDto> pageResponseDto = await m_workRepository.GetWorksAsync(workRequestDto);
        WorkPageResponse pageResponse = m_mapper.Map<WorkPageResponse>(pageResponseDto);

        pageResponse.CompletedOn = DateTimeOffset.Now;

        return pageResponse;
    }

    /// <inheritdoc />
    public async Task<WorkRelationship[]> GetWorkRelationshipsAsync(Guid workId, bool includeReverseRelationships = false)
    {
        WorkRelationshipDto[] workRelationshipDtoArray = await m_workRepository.GetWorkRelationshipsAsync(workId, includeReverseRelationships);
        WorkRelationship[] workRelationshipArray = m_mapper.Map<WorkRelationship[]>(workRelationshipDtoArray);
        return workRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<WorkToProductRelationship[]> GetWorkToProductRelationshipsAsync(Guid workId)
    {
        WorkToProductRelationshipDto[] workToProductRelationshipDtoArray = await m_workRepository.GetWorkToProductRelationshipsAsync(workId);
        WorkToProductRelationship[] workToProductRelationshipArray = m_mapper.Map<WorkToProductRelationship[]>(workToProductRelationshipDtoArray);
        return workToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<WorkToProductRelationship[]> GetWorkToProductRelationshipsByProductAsync(Guid productId)
    {
        WorkToProductRelationshipDto[] workToProductRelationshipDtoArray = await m_workRepository.GetWorkToProductRelationshipsByProductAsync(productId);
        WorkToProductRelationship[] workToProductRelationshipArray = m_mapper.Map<WorkToProductRelationship[]>(workToProductRelationshipDtoArray);
        return workToProductRelationshipArray;
    }

    /// <inheritdoc />
    public async Task<Work> CreateWorkAsync(Work work)
    {
        WorkDto workDto = m_mapper.Map<WorkDto>(work);
        WorkDto createdWorkDto = await m_workRepository.CreateWorkAsync(workDto);
        Work createdWork = m_mapper.Map<Work>(createdWorkDto);
        return createdWork;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateWorkAsync(Work work)
    {
        WorkDto workDto = m_mapper.Map<WorkDto>(work);
        var updated = await m_workRepository.UpdateWorkAsync(workDto);
        return updated;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteWorkAsync(Guid workId)
    {
        var deleted = await m_workRepository.DeleteWorkAsync(workId);
        return deleted;
    }
}
