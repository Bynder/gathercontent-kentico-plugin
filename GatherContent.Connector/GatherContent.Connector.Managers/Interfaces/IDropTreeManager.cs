namespace GatherContent.Connector.Managers.Interfaces
{
  using System.Collections.Generic;

  using GatherContent.Connector.Managers.Models.ImportItems;

  /// <summary>
  /// 
  /// </summary>
  public interface IDropTreeManager : IManager
  {
    List<DropTreeModel> GetChildrenNodes(string id);

    List<DropTreeModel> GetTopLevelNode(string id);
  }
}