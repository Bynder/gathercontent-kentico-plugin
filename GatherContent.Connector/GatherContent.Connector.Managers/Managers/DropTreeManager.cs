namespace GatherContent.Connector.Managers.Managers
{
    using System.Collections.Generic;
    using System.Linq;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.IRepositories.Interfaces;
    using GatherContent.Connector.IRepositories.Models.Import;
    using GatherContent.Connector.Managers.Interfaces;
    using GatherContent.Connector.Managers.Models.ImportItems;

    /// <summary>
    /// 
    /// </summary>
    public class DropTreeManager : IDropTreeManager
    {
        protected IAccountsRepository AccountsRepository;

        protected IDropTreeRepository DropTreeRepository;

        protected GCAccountSettings GcAccountSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropTreeManager"/> class. 
        /// 
        /// </summary>
        /// <param name="dropTreeRepository">
        /// </param>
        /// <param name="accountsRepository">
        /// </param>
        /// <param name="gcAccountSettings">
        /// </param>
        public DropTreeManager(IDropTreeRepository dropTreeRepository, IAccountsRepository accountsRepository, GCAccountSettings gcAccountSettings)
        {
            this.AccountsRepository = accountsRepository;
            this.GcAccountSettings = gcAccountSettings;
            this.DropTreeRepository = dropTreeRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DropTreeModel> GetChildrenNodes(string id)
        {
            var model = new List<DropTreeModel>();
            var items = this.DropTreeRepository.GetChildren(id);
            foreach (var cmsItem in items)
            {
                model.Add(new DropTreeModel { Title = cmsItem.Title, Key = cmsItem.Id, Icon = cmsItem.Icon, IsLazy = true });
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DropTreeModel> GetTopLevelNode(string id)
        {
            var model = new List<DropTreeModel>();
            var items = this.DropTreeRepository.GetHomeNode(id);

            if (string.IsNullOrEmpty(id) || id == "null")
            {
                foreach (var cmsItem in items)
                {
                    model.Add(new DropTreeModel { Title = cmsItem.Title, Key = cmsItem.Id, Icon = cmsItem.Icon, IsLazy = true });
                }
            }
            else
            {
                var dropTreeHomeNode = this.DropTreeRepository.GetHomeNodeId();

                foreach (var cmsItem in items)
                {
                    model.Add(
                        new DropTreeModel
                            {
                                Title = cmsItem.Title,
                                Key = cmsItem.Id,
                                Icon = cmsItem.Icon,
                                IsLazy = false,
                                Selected = id == dropTreeHomeNode,
                                Expanded = true,
                                Children = this.CreateChildrenTree(id, cmsItem.Children),
                            });
                }
            }

            return model;
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
                        var node = new DropTreeModel { Title = item.Title, Key = item.Id, IsLazy = true, Icon = item.Icon, Selected = true };
                        list.Add(node);
                    }
                    else
                    {
                        var node = new DropTreeModel { Title = item.Title, Key = item.Id, IsLazy = true, Icon = item.Icon, Selected = false };
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
                                       Children = this.CreateChildrenTree(id, item.Children),
                                       Expanded = true
                                   };
                    list.Add(node);
                }
            }

            return list;
        }
    }
}