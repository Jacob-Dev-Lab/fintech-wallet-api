using Wallet.Application.Common;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.Application.UseCases
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<Result<WalletAccount>> CreateWallet()
        {
            var wallet = new WalletAccount(Guid.NewGuid(), DateTime.UtcNow);

            await _walletRepository.Add(wallet);

            return Result<WalletAccount>.Success(wallet);
        }

        public async Task<Result<WalletAccount>> Deposit(Guid walletId, DepositRequest request)
        {
            try
            {
                var wallet = await _walletRepository.GetById(walletId);

                if (wallet is null)
                    return Result<WalletAccount>.Failure("Invalid wallet id.");

                wallet.Deposit(request.Amount);
                await _walletRepository.Update(wallet);

                return Result<WalletAccount>.Success(wallet);
            }
            catch (DomainException ex)
            {
                return Result<WalletAccount>.Failure(ex.Message);
            }
        }
    }}
