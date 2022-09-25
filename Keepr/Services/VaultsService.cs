using System;
using System.Collections.Generic;
using Keepr.Models;
using Keepr.Repositories;

namespace Keepr.Services
{
  public class VaultsService
  {
    private readonly VaultsRepository _vaultsRepo;

    public VaultsService(VaultsRepository vaultsRepo)
    {
      _vaultsRepo = vaultsRepo;
    }

    internal List<Vault> GetAllProfileVaults(string id, string userId)
    {
      List<Vault> vaults = _vaultsRepo.GetAllProfileVaults(userId);
      vaults = vaults.FindAll(v => v.IsPrivate == false || v.CreatorId == userId);
      return vaults;
    }

    internal Vault GetVaultById(int vaultId, string userId)
    {
      Vault vault = _vaultsRepo.GetVaultById(vaultId);
      if (vault == null){
        throw new Exception("no vault with that Id");
      }
      if (vault.CreatorId != userId){
        throw new Exception("This is not your vault!");
      }
      return vault;
    }

    internal Vault CreateVault(Vault vaultData)
    {
      return _vaultsRepo.CreateVault(vaultData);
    }

    internal Vault EditVault(Vault vaultData, Account user)
    {
      int vaultId = vaultData.Id;
      Vault original = GetVaultById(vaultId, user.Id);
      original.Name = vaultData.Name ?? original.Name;
      original.Description = vaultData.Description ?? original.Description;
      original.Img = vaultData.Img ?? original.Img;
      original.IsPrivate = vaultData.IsPrivate ?? original.IsPrivate;
      return _vaultsRepo.EditVault(original);
    }

    internal string DeleteVault(int id, Account user)
    {
      Vault vault = GetVaultById(id, user.Id);
      _vaultsRepo.DeleteVault(id);
      return $"Your vault: {vault.Name} has been permanently deleted";
    }
  }
}