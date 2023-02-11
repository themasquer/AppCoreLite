using AppCoreLite.Configs;
using AppCoreLite.Entities;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Results.Bases;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Services
{
    public class TreeNodeService : EntityService<TreeNode>
    {
        public TreeNodeServiceConfig TreeNodeServiceConfig { get; set; }
        public DbSet<TreeNodeDetail> DetailNodes { get; set; }
        public List<TreeNode> Nodes { get; set; }

        public TreeNodeService(DbContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            TreeNodeServiceConfig = new TreeNodeServiceConfig();
            DetailNodes = _db.Set<TreeNodeDetail>();
            Nodes = Query().ToList();
        }

        public override void Set(Languages language)
        {
            TreeNodeServiceConfig.Set(language);
            base.Set(language);
        }

        public override IQueryable<TreeNode> Query(params Expression<Func<TreeNode, object?>>[] navigationPropertyPaths)
        {
            return base.Query(q => q.TreeNodeDetail);
        }

        public virtual IQueryable<TreeNodeDetail> GetDetailNodesQuery()
        {
            return DetailNodes.Include(dn => dn.TreeNodes).AsQueryable();
        }

        public virtual IQueryable<TreeNodeDetail> GetDetailNodesQuery(int level)
        {
            return GetDetailNodesQuery().Where(q => q.Level == level).OrderBy(q => q.Level);
        }

        public virtual List<JqueryOrgchartNode> GetJqueryOrgchartNodes()
        {
            var query = Query();
            var filter = new TreeNodeFilter();
            return query.Select(q => new JqueryOrgchartNode()
            {
                id = q.Id,
                parent = q.ParentId,
                level = q.TreeNodeDetail.Level,
                name = (
                            filter.Language == Languages.Turkish ?
                                q.NameTurkish ?? "" :
                                q.NameEnglish ?? ""
                       )
                       + "<br />" +
                       (
                            filter.ShowDetailTexts ?
                            (
                                filter.Language == Languages.Turkish ?
                                    q.TreeNodeDetail.TextTurkish ?? "" :
                                    q.TreeNodeDetail.TextEnglish ?? ""
                            ) :
                            (
                                filter.Language == Languages.Turkish ?
                                    q.TextTurkish ?? "" :
                                    q.TextEnglish ?? ""
                            )
                        )
                        + "<br />" +
                        (
                            filter.ShowAbbreviations ?
                            (
                                filter.Language == Languages.Turkish ?
                                    q.AbbreviationTurkish ?? "" :
                                    q.AbbreviationEnglish ?? ""
                            ) : ""
                        )
            }).ToList();
        }

        public virtual List<JqueryOrgchartNode> GetJqueryOrgchartNodes(TreeNodeFilter filter)
        {
            var query = Query();
            if (filter.ShowOnlyActive)
                query = query.Where(q => q.IsActive);
            return query.Select(q => new JqueryOrgchartNode()
            {
                id = q.Id,
                parent = q.ParentId,
                level = q.TreeNodeDetail.Level,
                name = (
                            filter.Language == Languages.Turkish ?
                                q.NameTurkish ?? "" :
                                q.NameEnglish ?? ""
                       )
                       + "<br />" +
                       (
                            filter.ShowDetailTexts ?
                            (
                                filter.Language == Languages.Turkish ?
                                    q.TreeNodeDetail.TextTurkish ?? "" :
                                    q.TreeNodeDetail.TextEnglish ?? ""
                            ) :
                            (
                                filter.Language == Languages.Turkish ?
                                    q.TextTurkish ?? "" :
                                    q.TextEnglish ?? ""
                            )
                        )
                        + "<br />" +
                        (
                            filter.ShowAbbreviations ?
                            (
                                filter.Language == Languages.Turkish ?
                                    q.AbbreviationTurkish ?? "" :
                                    q.AbbreviationEnglish ?? ""
                            ) : ""
                        )
            }).ToList();
        }

        public virtual List<TreeNode> GetNodes(int parentId = 0)
        {
            List<TreeNode>? nodes = null;
            var recursiveNodes = GetRecursiveNodes(parentId);
            return GetNodes(recursiveNodes, nodes);
        }

        public virtual List<TreeNode> GetNodesByLevel(int level)
        {
            List<TreeNode>? nodes = GetNodes();
            return nodes.Where(n => n.TreeNodeDetail.Level == level).ToList();
        }

        public virtual List<TreeNodeRecursive> GetRecursiveNodes(int parentId = 0)
        {
            return Nodes.Where(n => n.ParentId == parentId).Select(n => new TreeNodeRecursive()
            {
                Id = n.Id,
                ParentId = n.ParentId,
                AbbreviationEnglish = n.AbbreviationEnglish,
                AbbreviationTurkish = n.AbbreviationTurkish,
                Guid = n.Guid,
                IsActive = n.IsActive,
                NameEnglish = n.NameEnglish,
                NameTurkish = n.NameTurkish,
                TextEnglish = n.TextEnglish,
                TextTurkish = n.TextTurkish,
                TreeNodeDetailId = n.TreeNodeDetailId,
                Nodes = GetRecursiveNodes(n.Id),
                CreateDate = n.CreateDate,
                CreatedBy = n.CreatedBy,
                UpdateDate = n.UpdateDate,
                UpdatedBy = n.UpdatedBy
            }).ToList();
        }

        public virtual List<TreeNodeDetail> GetDetailNodes(int level)
        {
            return GetDetailNodesQuery(level).ToList();
        }

        public virtual TreeNodeDetail GetDetailNode(int id)
        {
            return GetDetailNodesQuery().SingleOrDefault(q => q.Id == id);
        }

        public override Result Add(TreeNode entity, bool save = true, bool trim = true)
        {
            if (entity.TreeNodeDetail == null)
                return Error(TreeNodeServiceConfig.NodeDetailNotFound + " " + Config.OperationFailed);
            if (string.IsNullOrWhiteSpace(entity.NameEnglish) && string.IsNullOrWhiteSpace(entity.NameTurkish))
                return Error(TreeNodeServiceConfig.NodeNameRequired + " " + Config.OperationFailed);
            Expression<Func<TreeNode, bool>> predicate = p => p.Id != entity.Id;
            Expression<Func<TreeNode, bool>> namePredicate;
            if (!string.IsNullOrWhiteSpace(entity.NameEnglish))
            {
                namePredicate = p => (p.NameEnglish ?? "").ToLower() == (entity.NameEnglish ?? "").ToLower();
                if (!string.IsNullOrWhiteSpace(entity.NameTurkish))
                {
                    namePredicate = namePredicate.Or(p => (p.NameTurkish ?? "").ToLower() == (entity.NameTurkish ?? "").ToLower());
                }
            }
            else
            {
                namePredicate = p => (p.NameTurkish ?? "").ToLower() == (entity.NameTurkish ?? "").ToLower();
            }
            predicate = predicate.And(namePredicate);
            var result = base.ItemExists(predicate);
            if (result.IsSuccessful)
                return Error(TreeNodeServiceConfig.NodeFound + " " + Config.OperationFailed);
            var entityNode = new TreeNode()
            {
                Id = base.GetMaxId(true) + 1,
                AbbreviationEnglish = entity.AbbreviationEnglish,
                AbbreviationTurkish = entity.AbbreviationTurkish,
                IsActive = entity.IsActive,
                NameEnglish = entity.NameEnglish,
                NameTurkish = entity.NameTurkish,
                ParentId = entity.ParentId,
                TextEnglish = entity.TextEnglish,
                TextTurkish = entity.TextTurkish
            };
            if (entity.TreeNodeDetailId > 0)
            {
                entityNode.TreeNodeDetailId = entity.TreeNodeDetailId;
                if (!string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextEnglish) || !string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextTurkish))
                {
                    result = UpdateDetailNode(entity.TreeNodeDetail);
                    if (!result.IsSuccessful)
                        return result;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextEnglish) && string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextTurkish))
                    return Error(TreeNodeServiceConfig.NodeDetailTextRequired + " " + Config.OperationFailed);
                result = AddDetailNode(entityNode, entity.TreeNodeDetail);
                if (!result.IsSuccessful)
                    return result;
            }
            result = base.Add(entityNode, save, trim);
            if (!result.IsSuccessful)
                return result;
            entity.Id = entityNode.Id;
            return Success();
        }

        public override Result Update(TreeNode entity, bool save = true, bool trim = true)
        {
            if (entity.TreeNodeDetail == null)
                return Error(TreeNodeServiceConfig.NodeDetailNotFound + " " + Config.OperationFailed);
            if (string.IsNullOrWhiteSpace(entity.NameEnglish) && string.IsNullOrWhiteSpace(entity.NameTurkish))
                return Error(TreeNodeServiceConfig.NodeNameRequired + " " + Config.OperationFailed);
            Expression<Func<TreeNode, bool>> predicate = p => p.Id != entity.Id;
            Expression<Func<TreeNode, bool>> namePredicate;
            if (!string.IsNullOrWhiteSpace(entity.NameEnglish))
            {
                namePredicate = p => (p.NameEnglish ?? "").ToLower() == (entity.NameEnglish ?? "").ToLower();
                if (!string.IsNullOrWhiteSpace(entity.NameTurkish))
                {
                    namePredicate = namePredicate.Or(p => (p.NameTurkish ?? "").ToLower() == (entity.NameTurkish ?? "").ToLower());
                }
            }
            else
            {
                namePredicate = p => (p.NameTurkish ?? "").ToLower() == (entity.NameTurkish ?? "").ToLower();
            }
            predicate = predicate.And(namePredicate);
            var result = base.ItemExists(predicate);
            if (result.IsSuccessful)
                return Error(TreeNodeServiceConfig.NodeFound + " " + Config.OperationFailed);
            var entityNode = base.GetItem(n => n.Id == entity.Id);
            if (entityNode == null)
                return Error(TreeNodeServiceConfig.NodeNotFound + " " + Config.OperationFailed);
            var entityNodes = new List<TreeNode>();
            if (entityNode.IsActive != entity.IsActive)
            {
                if (entityNode.ParentId == 0)
                    return Error(TreeNodeServiceConfig.NoUpdateForRootNodeActive + " " + Config.OperationFailed);
                entityNodes = Query().Where(q => GetNodes(entityNode.Id).Select(en => en.Id).Contains(q.Id)).ToList();
                foreach (var en in entityNodes)
                {
                    en.IsActive = entity.IsActive;
                }
            }
            entityNode.AbbreviationEnglish = entity.AbbreviationEnglish;
            entityNode.AbbreviationTurkish = entity.AbbreviationTurkish;
            entityNode.IsActive = entity.IsActive;
            entityNode.NameEnglish = entity.NameEnglish;
            entityNode.NameTurkish = entity.NameTurkish;
            entityNode.ParentId = entity.ParentId;
            entityNode.TextEnglish = entity.TextEnglish;
            entityNode.TextTurkish = entity.TextTurkish;
            if (entity.TreeNodeDetailId > 0)
            {
                entityNode.TreeNodeDetailId = entity.TreeNodeDetailId;
                if (!string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextEnglish) || !string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextTurkish))
                {
                    result = UpdateDetailNode(entity.TreeNodeDetail);
                    if (!result.IsSuccessful)
                        return result;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextEnglish) && string.IsNullOrWhiteSpace(entity.TreeNodeDetail.TextTurkish))
                    return Error(TreeNodeServiceConfig.NodeDetailTextRequired + " " + Config.OperationFailed);
                result = AddDetailNode(entityNode, entity.TreeNodeDetail);
                if (!result.IsSuccessful)
                    return result;
            }
            entityNodes.Add(entityNode);
            foreach (var en in entityNodes)
            {
                base.Update(en, false);
            }
            base.Save();
            return Success();
        }

        public override Result Delete(int id, bool save = true)
        {
            var entityNode = base.GetItem(id);
            if (entityNode == null)
                return Error(TreeNodeServiceConfig.NodeNotFound + " " + Config.OperationFailed);
            if (entityNode.ParentId == 0)
                return Error(TreeNodeServiceConfig.NoDeleteForRootNode + " " + Config.OperationFailed);
            var entityNodes = Query().Where(q => GetNodes(entityNode.Id).Select(en => en.Id).Contains(q.Id)).ToList();
            entityNodes.Add(entityNode);
            foreach (var en in entityNodes)
            {
                base.Delete(en, false);
            }
            base.Save();
            return Success();
        }

        public virtual Result AddDetailNode(TreeNode node, TreeNodeDetail detailNode)
        {
            Expression<Func<TreeNodeDetail, bool>> predicate = p => p.Id != detailNode.Id;
            Expression<Func<TreeNodeDetail, bool>> textPredicate;
            if (!string.IsNullOrWhiteSpace(detailNode.TextEnglish))
            {
                textPredicate = p => (p.TextEnglish ?? "").ToLower() == (detailNode.TextEnglish ?? "").ToLower();
                if (!string.IsNullOrWhiteSpace(detailNode.TextTurkish))
                {
                    textPredicate = textPredicate.Or(p => (p.TextTurkish ?? "").ToLower() == (detailNode.TextTurkish ?? "").ToLower());
                }
            }
            else
            {
                textPredicate = p => (p.TextTurkish ?? "").ToLower() == (detailNode.TextTurkish ?? "").ToLower();
            }
            predicate = predicate.And(textPredicate);
            if (GetDetailNodesQuery().Any(predicate))
            {
                return Error(TreeNodeServiceConfig.NodeDetailFound + " " + Config.OperationFailed);
            }
            node.TreeNodeDetail = new TreeNodeDetail()
            {
                Level = detailNode.Level,
                TextEnglish = detailNode.TextEnglish?.Trim(),
                TextTurkish = detailNode.TextTurkish?.Trim()
            };
            return Success();
        }

        public virtual Result UpdateDetailNode(TreeNodeDetail detailNode)
        {
            Expression<Func<TreeNodeDetail, bool>> predicate = p => p.Id != detailNode.Id;
            Expression<Func<TreeNodeDetail, bool>> textPredicate;
            if (!string.IsNullOrWhiteSpace(detailNode.TextEnglish))
            {
                textPredicate = p => (p.TextEnglish ?? "").ToLower() == (detailNode.TextEnglish ?? "").ToLower();
                if (!string.IsNullOrWhiteSpace(detailNode.TextTurkish))
                {
                    textPredicate = textPredicate.Or(p => (p.TextTurkish ?? "").ToLower() == (detailNode.TextTurkish ?? "").ToLower());
                }
            }
            else
            {
                textPredicate = p => (p.TextTurkish ?? "").ToLower() == (detailNode.TextTurkish ?? "").ToLower();
            }
            predicate = predicate.And(textPredicate);
            if (GetDetailNodesQuery().Any(predicate))
            {
                return Error(TreeNodeServiceConfig.NodeDetailFound + " " + Config.OperationFailed);
            }
            var entityDetailNode = GetDetailNode(detailNode.Id);
            if (entityDetailNode == null)
                return Error(TreeNodeServiceConfig.NodeDetailNotFound + " " + Config.OperationFailed);
            entityDetailNode.Level = detailNode.Level;
            entityDetailNode.TextEnglish = detailNode.TextEnglish?.Trim();
            entityDetailNode.TextTurkish = detailNode.TextTurkish?.Trim();
            DetailNodes.Update(entityDetailNode);
            return Success();
        }

        public virtual Result DeleteDetailNode(int id)
        {
            var detailNode = GetDetailNode(id);
            if (detailNode == null)
                return Error(TreeNodeServiceConfig.NodeDetailNotFound + " " + Config.OperationFailed);
            if (detailNode.TreeNodes != null && detailNode.TreeNodes.Count > 0)
                return Error(TreeNodeServiceConfig.NoDeleteForNodeDetailHasNodes + " " + Config.OperationFailed);
            DetailNodes.Remove(detailNode);
            _db.SaveChanges();
            return Success(TreeNodeServiceConfig.NodeDetailDeleted);
        }

        private List<TreeNode> GetNodes(List<TreeNodeRecursive> recursiveNodes, List<TreeNode>? nodes = null)
        {
            if (nodes == null)
                nodes = new List<TreeNode>();
            TreeNode node;
            foreach (var recursiveNode in recursiveNodes)
            {
                node = Nodes.FirstOrDefault(n => n.Id == recursiveNode.Id);
                nodes.Add(node);
                GetNodes(recursiveNode.Nodes, nodes);
            }
            return nodes;
        }
    }
}
