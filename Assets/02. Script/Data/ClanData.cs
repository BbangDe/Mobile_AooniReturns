using PlayFab;
using PlayFab.GroupsModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClanData : Manager
{
    public void GetClanName(Action<string> onSuccess, Action<EPlayerDataError> onError)
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan))
        {
            onSuccess?.Invoke(string.Empty);
            return;
        }

        var request = new GetGroupRequest
        {
            Group = new EntityKey
            {
                Id = App.Data.Player.Clan,
                Type = "group"
            }
        };

        PlayFabGroupsAPI.GetGroup(request,
        result =>
        {
            string clanName = result.GroupName;
            Debug.Log($"Group Name: {clanName}");
            onSuccess?.Invoke(clanName);
        },
        error =>
        {
            Debug.LogError($"Failed to get clan name. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.GetGroupFailed);
        });
    }

    public void CheckPlayerIsClanOwner(Action<bool> onSuccess, Action<EPlayerDataError> onError)
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan))
        {
            onSuccess?.Invoke(false);
            return;
        }

        var request = new ListGroupMembersRequest
        {
            Group = new EntityKey 
            { 
                Id = App.Data.Player.Clan, 
                Type = "group" 
            }
        };

        PlayFabGroupsAPI.ListGroupMembers(request,
        result =>
        {
            string adminRoleId = "admins";

            foreach (var member in result.Members)
            {
                if (member.RoleId == adminRoleId)
                {
                    string ownerId = member.Members[0].Key.Id;
                    var isOwner = ownerId.Equals(PlayFabSettings.staticPlayer.EntityId);
                    Debug.Log($"Clan Owner Entity ID: {ownerId}");
                    onSuccess?.Invoke(isOwner);
                    return;
                }
            }
        },
        error =>
        {
            Debug.LogError($"Failed to get clan members. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.ListGroupMembersFailed);
        });
    }

    public void CreateClan(string clanName, Action<string> onSuccess, Action<EPlayerDataError> onError)
    {
        if (!string.IsNullOrEmpty(App.Data.Player.Clan)) return;

        var request = new CreateGroupRequest
        {
            GroupName = clanName
        };

        PlayFabGroupsAPI.CreateGroup(request,
        result =>
        {
            string groupId = result.Group.Id;
            Debug.Log($"Clan '{clanName}' created successfully with Group ID: {groupId}");
            App.Data.Player.SetClan(groupId, null, null);

            onSuccess?.Invoke(groupId);
        },
        error =>
        {
            Debug.LogError($"Failed to create clan. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.CreateGroupFailed);
        });
    }

    public void JoinClan(string groupId, Action onSuccess, Action<EPlayerDataError> onError)
    {
        if (!string.IsNullOrEmpty(App.Data.Player.Clan)) return;

        var request = new AddMembersRequest
        {
            Group = new EntityKey { Id = groupId, Type = "group" },
            Members = new List<EntityKey> { new EntityKey { Id = PlayFabSettings.staticPlayer.EntityId, Type = "title_player_account" } }
        };

        PlayFabGroupsAPI.AddMembers(request,
        result =>
        {
            Debug.Log("Successfully joined the clan.");
            App.Data.Player.SetClan(groupId, null, null);
            onSuccess?.Invoke();
        },
        error =>
        {
            Debug.LogError($"Failed to join the clan. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.JoinGroupFailed);
        });
    }

    public void LeaveClan(Action onSuccess, Action<EPlayerDataError> onError)
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan)) return;

        var request = new RemoveMembersRequest
        {
            Group = new EntityKey { Id = App.Data.Player.Clan, Type = "group" },
            Members = new List<EntityKey> { new EntityKey { Id = PlayFabSettings.staticPlayer.EntityId, Type = "title_player_account" } }
        };

        PlayFabGroupsAPI.RemoveMembers(request,
        result =>
        {
            Debug.Log("Successfully left the clan.");
            App.Data.Player.SetClan(string.Empty, null, null);
            onSuccess?.Invoke();
        },
        error =>
        {
            Debug.LogError($"Failed to leave the clan. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.LeaveGroupFailed);
        });
    }

    public void DeleteClan(Action onSuccess, Action<EPlayerDataError> onError)
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan)) return;

        var request = new DeleteGroupRequest
        {
            Group = new EntityKey
            {
                Id = App.Data.Player.Clan,
                Type = "group"
            }
        };

        PlayFabGroupsAPI.DeleteGroup(request,
        result =>
        {
            App.Data.Player.SetClan(string.Empty, null, null);
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.DeleteGroupFailed);
        });
    }

    public void GetClanMembers(Action<List<EntityMemberRole>> onSuccess, Action<EPlayerDataError> onError)
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan)) return;

        var request = new ListGroupMembersRequest
        {
            Group = new EntityKey 
            { 
                Id = App.Data.Player.Clan, 
                Type = "group" 
            }
        };

        PlayFabGroupsAPI.ListGroupMembers(request,
        result =>
        {
            onSuccess?.Invoke(result.Members);
        },
        error =>
        {
            Debug.LogError($"Failed to get clan members. Error: {error.GenerateErrorReport()}");
            onError?.Invoke(EPlayerDataError.ListGroupMembersFailed);
        });
    }
}
