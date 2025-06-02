using DAL.Entities.Users;
using DTO.Users;
using Mapster;

namespace BLL.Users;

public sealed class RoleDtoService
{
    private RoleService Service { get; }

    public RoleDtoService(RoleService service)
    {
        Service = service;
    }

    public async Task<string> Add(RoleDto.AddIn dto)
    {
        return (await Service.Add(dto)).Id;
    }

    public async Task<IEnumerable<RoleDto.ListOut>> List()
    {
        var cnf = new TypeAdapterConfig();
        cnf.NewConfig<Role, RoleDto.ListOut>()
            .Map(x => x.IsDeletable, x => !(x.Users.Any() || x.Claims.Any()));

        return await Service.List<RoleDto.ListOut>(x => x.OrderBy(x => x.Name), cnf);
    }

    public async Task<RoleDto.ByIdOut> ById(string id)
    {
        return await Service.ById<RoleDto.ByIdOut>(id);
    }

    public async Task Edit(RoleDto.EditIn dto)
    {
        await Service.Edit(dto);
    }

    public async Task Delete(string id)
    {
        await Service.Delete(id);
    }
}
