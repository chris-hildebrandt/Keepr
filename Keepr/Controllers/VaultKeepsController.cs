using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeWorks.Auth0Provider;
using Keepr.Models;
using Keepr.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Keepr.Controllers
{
  [ApiController]
    [Route("api/[controller]")]
    public class VaultKeepsController : ControllerBase
    {
        private readonly VaultKeepsService _vaultKeepsService;
        private readonly VaultsService _vaultsService;

    public VaultKeepsController(VaultKeepsService vaultKeepsService, VaultsService vaultsService)
    {
      _vaultKeepsService = vaultKeepsService;
      _vaultsService = vaultsService;
    }

    // I never need to get vaultkeeps directly, i just use them to join vaults to keeps, so my only cruds are create and delete

    [HttpGet]
    public ActionResult<VaultKeep> GetAll(){
      try
      {
        List<VaultKeep> vaultKeeps = _vaultKeepsService.GetAll();
        return Ok(vaultKeeps);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<VaultKeep>> GetById(int id)
    {
      try
      {
        Account user = await HttpContext.GetUserInfoAsync<Account>();
        VaultKeep vaultKeep = _vaultKeepsService.GetById(id, user);
        return Ok(vaultKeep);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    // the body needs to include the vaultId and the keepId
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<VaultKeep>> AddKeepToVault([FromBody] VaultKeep vaultKeepData){
        try
      {
        Account user = await HttpContext.GetUserInfoAsync<Account>();
        vaultKeepData.CreatorId = user.Id;
        Vault vault = _vaultsService.GetVaultById(vaultKeepData.VaultId, user.Id);
        VaultKeep vaultKeep = _vaultKeepsService.AddKeepToVault(vaultKeepData, vault);
        return Ok(vaultKeep);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<string>> RemoveKeepFromVault(int id){
        try
      {
        Account user = await HttpContext.GetUserInfoAsync<Account>();
        string message = _vaultKeepsService.RemoveKeepFromVault(id, user);
        return Ok(message);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}