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
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ProfileService(IMapper mapper,
        IUsersRepository usersRepository,
        ISubscriptionsRepository subscriptionsRepository)
    {
        _mapper = mapper;
        _usersRepository = usersRepository;
        this._subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ProfilePrivateData> GatherProfileData(ApplicationUser requestingUser)
    {
        var userData = await _usersRepository.GetByIdAsync(requestingUser.Id);

        var result = _mapper.Map<ApplicationUser, ProfilePrivateData>(userData);

        result.Subscription = await _subscriptionsRepository.GetSubscriptionByCustomerId(userData.CustomerId);

        return result;
    }
}