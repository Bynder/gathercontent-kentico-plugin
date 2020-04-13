namespace GatherContent.Connector.Managers.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.Managers.Models.ImportItems;

    /// <summary>
    /// 
    /// </summary>
    public interface IDropTreeManager : IManager
    {
        /// <summary>
        /// The get children nodes.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<DropTreeModel> GetChildrenNodes(string id);

        /// <summary>
        /// The get top level node.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<DropTreeModel> GetTopLevelNode(string id);
    }
}