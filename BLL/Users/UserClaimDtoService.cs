using Common.Extensions;
using DAL.Entities.Users;
using DTO.Users;
using Mapster;

namespace BLL.Users;

public sealed class UserClaimDtoService
{
    private UserClaimService Service { get; }

    private RoleClaimService RoleClaimService { get; }

    private UserService UserService { get; }

    public UserClaimDtoService(UserClaimService service, RoleClaimService roleClaimService, UserService userService)
    {
        Service = service;
        RoleClaimService = roleClaimService;
        UserService = userService;
    }

    public async Task<int> Add(UserClaimDto.AddIn dto)
    {
        return (await Service.Add(dto)).Id;
    }

    public async Task<IEnumerable<UserClaimDto.ListOut>> List(string userId)
    {
        var cnf = new TypeAdapterConfig();
        cnf.NewConfig<UserClaim, UserClaimDto.ListOut>()
            .Map(x => x.IsDeletable, x => true);

        return await Service.List<UserClaimDto.ListOut>(userId, cnf);
    }

    public async Task<UserClaimDto.ByIdOut> ById(int id)
    {
        return await Service.ById<UserClaimDto.ByIdOut>(id);
    }

    public async Task Edit(UserClaimDto.EditIn dto)
    {
        await Service.Edit(dto);
    }

    public async Task Delete(int id)
    {
        await Service.Delete(id);
    }

    public async Task Reset(string id)
    {
        var roleClaims = (await RoleClaimService.List<ClaimDto.Default>(await UserService.RoleIds(id))).ToList();
        var userClaims = (await Service.List<ClaimDto.Default>(id)).ToList(); 

        var (add, del) = userClaims.Merge(roleClaims, x => $"{x.ClaimType}|{x.ClaimValue}");

        await Service.Add(add.Select(x => (id, x.ClaimType, x.ClaimValue)));
        await Service.Delete(del.Select(x => x.Id));
    }
}
