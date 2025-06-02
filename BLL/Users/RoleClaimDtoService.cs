using DAL.Entities.Users;
using DTO.Users;
using Mapster;

namespace BLL.Users;

public sealed class RoleClaimDtoService
{
    private RoleClaimService Service { get; }

    private RoleService RoleService { get; }

    private UserClaimDtoService UserClaimDtoService { get; }

    public RoleClaimDtoService(RoleClaimService service, RoleService roleService, UserClaimDtoService userClaimDtoService)
    {
        Service = service;
        RoleService = roleService;
        UserClaimDtoService = userClaimDtoService;
    }

    public async Task<int> Add(RoleClaimDto.AddIn dto)
    {
        return (await Service.Add(dto)).Id;
    }

    public async Task<IEnumerable<RoleClaimDto.ListOut>> List(string roleId)
    {
        var cnf = new TypeAdapterConfig();
        cnf.NewConfig<RoleClaim, RoleClaimDto.ListOut>()
            .Map(x => x.IsDeletable, x => true);

        return await Service.List<RoleClaimDto.ListOut>(roleId, cnf);
    }

    public async Task<RoleClaimDto.ByIdOut> ById(int id)
    {
        return await Service.ById<RoleClaimDto.ByIdOut>(id);
    }

    public async Task Edit(RoleClaimDto.EditIn dto)
    {
        await Service.Edit(dto);
    }

    public async Task Delete(int id)
    {
        await Service.Delete(id);
    }

    public async Task Reset(string id)
    {
        foreach (var x in await RoleService.Users(id))
            await UserClaimDtoService.Reset(x);
    }
}
