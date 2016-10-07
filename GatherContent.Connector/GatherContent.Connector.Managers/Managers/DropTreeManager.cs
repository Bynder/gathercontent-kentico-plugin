using System.Linq;
using GatherContent.Connector.Entities;
using GatherContent.Connector.IRepositories.Models.Import;
using GatherContent.Connector.Managers.Models.ImportItems;
using System.Collections.Generic;
using GatherContent.Connector.IRepositories.Interfaces;
using GatherContent.Connector.Managers.Interfaces;

namespace GatherContent.Connector.Managers.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class DropTreeManager : IDropTreeManager
    {
        protected IDropTreeRepository DropTreeRepository;
        protected IAccountsRepository AccountsRepository;

        protected GCAccountSettings GcAccountSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dropTreeRepository"></param>
        /// <param name="accountsRepository"></param>
        /// <param name="gcAccountSettings"></param>
        public DropTreeManager(IDropTreeRepository dropTreeRepository, IAccountsRepository accountsRepository, GCAccountSettings gcAccountSettings)
        {
            AccountsRepository = accountsRepository;
            GcAccountSettings = gcAccountSettings;
            DropTreeRepository = dropTreeRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<DropTreeModel> CreateChildrenTree(string id, List<CmsItem> items)
        {
            var list = new List<DropTreeModel>();

            if (items.Select(i => i.Id.ToString()).Contains(id))
            {
                foreach (var item in items)
                {
                    if (id == item.Id)
                    {
                        var node = new DropTreeModel
                        {
                            Title = item.Title,
                            Key = item.Id,
                            IsLazy = true,
                            Icon = item.Icon,
                            Selected = true
                        };
                        list.Add(node);
                    }
                    else
                    {
                        var node = new DropTreeModel
                        {
                            Title = item.Title,
                            Key = item.Id,
                            IsLazy = true,
                            Icon = item.Icon,
                            Selected = false
                        };
                        list.Add(node);
                    }
                }
            }
            else
            {
                foreach (var item in items)
                {
                    var node = new DropTreeModel
                    {
                        Title = item.Title,
                        Key = item.Id,
                        IsLazy = false,
                        Icon = item.Icon,
                        Selected = false,
                        Children = CreateChildrenTree(id, item.Children),
                        Expanded = true
                    };
                    list.Add(node);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DropTreeModel> GetTopLevelNode(string id)
        {
            var model = new List<DropTreeModel>();
            var items = DropTreeRepository.GetHomeNode(id);

            if (string.IsNullOrEmpty(id) || id == "null")
            {
                foreach (var cmsItem in items)
                {
                    model.Add(new DropTreeModel
                    {
                        Title = cmsItem.Title,
                        Key = cmsItem.Id,
                        Icon = cmsItem.Icon,
                        IsLazy = true
                    });
                }
            }
            else
            {
             
               var dropTreeHomeNode = DropTreeRepository.GetHomeNodeId();

                foreach (var cmsItem in items)
                {
                    model.Add(new DropTreeModel
                    {
                        Title = cmsItem.Title,
                        Key = cmsItem.Id,
                        Icon = cmsItem.Icon,
                        IsLazy = false,
                        Selected = id == dropTreeHomeNode,
                        Expanded = true,
                        Children = CreateChildrenTree(id, cmsItem.Children),
                    });
                }
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DropTreeModel> GetChildrenNodes(string id)
        {
            var model = new List<DropTreeModel>();
            var items = DropTreeRepository.GetChildren(id);
            foreach (var cmsItem in items)
            {
                model.Add(new DropTreeModel
                {
                    Title = cmsItem.Title,
                    Key = cmsItem.Id,
                    Icon = cmsItem.Icon,
                    IsLazy = true
                });
            }

            return model;
        }
    }
}
