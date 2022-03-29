using System.Threading.Tasks;
using AutoMapper;
using Stocks.General.Models.MyProfile;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.MyProfile;

public class ProfileService : IProfileService
{
    private readonly IMapper _mapper;
    private readonly IUsersRepository _usersRepository;

    public ProfileService(IMapper mapper,
        IUsersRepository usersRepository)
    {
        _mapper = mapper;
        _usersRepository = usersRepository;
    }

    public async Task<ProfilePrivateData> GatherProfileData(ApplicationUser requestingUser)
    {
        var userData = await _usersRepository.GetByIdAsync(requestingUser.Id);

        return _mapper.Map<ApplicationUser, ProfilePrivateData>(userData);
    }
}