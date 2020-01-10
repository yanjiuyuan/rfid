using Common.ClassChange;
using Common.DTChange;
using Common.Excel;
using Common.Flie;
using Common.Ionic;
using Common.PDF;
using DingTalk.App_Start;
using DingTalk.Bussiness.FlowInfo;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    /// <summary>
    /// 项目结题
    /// </summary>
    [RoutePrefix("ProjectClosure")]
    public class ProjectClosureController : ApiController
    {
        /// <summary>
        /// 项目结题保存
        /// </summary>
        /// <param name="projectClosureModel"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public object Save([FromBody] ProjectClosureModel projectClosureModel)
        {
            try
            {
                using (DDContext dDContext = new DDContext())
                {
                    if (projectClosureModel.detailedLists.Count > 0)
                    {
                        dDContext.DetailedList.AddRange(projectClosureModel.detailedLists);
                    }
                    if (projectClosureModel.applicationUnitList.Count > 0)
                    {
                        dDContext.ApplicationUnit.AddRange(projectClosureModel.applicationUnitList);
                    }
                    if (projectClosureModel.projectFundingList.Count > 0)
                    {
                        dDContext.ProjectFunding.AddRange(projectClosureModel.projectFundingList);
                    }
                    if (projectClosureModel.longitudinalProject.Count > 0)
                    {
                        dDContext.LongitudinalProject.AddRange(projectClosureModel.longitudinalProject);
                    }
                    dDContext.ProjectClosure.Add(projectClosureModel.projectClosure);
                    dDContext.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "保存成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 项目结题读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public object Read(string taskId)
        {
            try
            {
                ProjectClosureModel projectClosureModel = new ProjectClosureModel();
                DDContext dDContext = new DDContext();
                ProjectClosure projectClosure = dDContext.ProjectClosure.Where(p => p.TaskId == taskId).FirstOrDefault();
                List<DetailedList> detailedLists = dDContext.DetailedList.Where(d => d.TaskId == taskId).ToList();
                List<ApplicationUnit> applicationUnitList = dDContext.ApplicationUnit.Where(d => d.TaskId == taskId).ToList();
                List<ProjectFunding> projectFundingList = dDContext.ProjectFunding.Where(d => d.TaskId == taskId).ToList();
                List<LongitudinalProject> longitudinalProjects = dDContext.LongitudinalProject.Where(d => d.TaskId == taskId).ToList();

                projectClosureModel.projectClosure = projectClosure;
                projectClosureModel.detailedLists = detailedLists;
                projectClosureModel.applicationUnitList = applicationUnitList;
                projectClosureModel.projectFundingList = projectFundingList;
                projectClosureModel.longitudinalProject = longitudinalProjects;
                
                return new NewErrorModel()
                {
                    data = projectClosureModel,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 立项书或建议书、PPT数据读取
        /// </summary>
        /// <param projectId="">项目Id</param>
        /// <returns></returns>
        [Route("ReadDefault")]
        [HttpGet]
        public NewErrorModel ReadDefault(string projectId)
        {
            try
            {
                DDContext context = new DDContext();

                ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == projectId).FirstOrDefault();
                if (projectInfo != null)
                {
                    if (projectInfo.ProjectState != "在研")
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "只有在研项目才能发起！", "") { },
                        };
                    }
                }
                //立项数据(附件)
                string FlowId = context.Flows.Where(t => t.FlowName == "立项申请").First().FlowId.ToString();
                List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId(FlowId);
                List<Tasks> tasksListQuery = tasksList.Where(t => t.FlowId.ToString() == FlowId && t.NodeId == 0).ToList();
                CreateProject createProject = context.CreateProject.Where(c => c.ProjectId == projectId).FirstOrDefault();
                if (createProject != null)
                {
                    Tasks tasks = tasksListQuery.Where(t => t.TaskId.ToString() == createProject.TaskId).FirstOrDefault();
                    if (tasks == null)
                    {
                        return new NewErrorModel()
                        {
                            data=null,
                            error = new Error(1, "该流程没有走过立项申请的数据！", "") { },
                        };
                    }
                    FileUrlModel fileUrlModel = new FileUrlModel()
                    {
                        fileName = tasks.OldFileUrl,
                        fileUrl = tasks.FileUrl,
                        mediaId = tasks.MediaId,
                    };
                    return new NewErrorModel()
                    {
                        data = fileUrlModel,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        data = "",
                        error = new Error(0, "暂无数据！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 项目采购清单、借用清单、领料清单、入库清单、借用清单、维修清单、受理知识产权清单  流程数据读取
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("ReadFlowInfo")]
        [HttpGet]
        public NewErrorModel ReadFlowInfo(string projectId)
        {
            try
            {
                DDContext context = new DDContext();
                List<FlowModel> FlowModelList = new List<FlowModel>();
                List<Flows> flowsList = context.Flows.Where(t => t.FlowName.Contains("采购") || t.FlowName.Contains("借入") || t.FlowName.Contains("知识产权") || t.FlowName.Contains("入库") || t.FlowName.Contains("领料")).ToList();
                foreach (var flows in flowsList)
                {
                    string FlowId = flows.FlowId.ToString();
                    string FlowName = flows.FlowName.ToString();
                    List<Tasks> tasksList = FlowInfoServer.ReturnUnFinishedTaskId(FlowId).Where(t => t.ProjectId == projectId).ToList();
                    List<Tasks> tasksListQuery = tasksList.Where(t => t.FlowId.ToString() == FlowId && t.NodeId == 0).ToList();
                    ProjectInfo createProject = context.ProjectInfo.Where(c => c.ProjectId == projectId).FirstOrDefault();
                    if (createProject != null)
                    {
                        foreach (var item in tasksListQuery)
                        {
                            FlowModelList.Add(new FlowModel()
                            {
                                Type = FlowName,
                                ApplyMan = item.ApplyMan,
                                ApplyManId = item.ApplyManId,
                                ApplyTime = item.ApplyTime,
                                OldtaskId = item.TaskId.ToString()
                            });
                        }
                    }
                }
                return new NewErrorModel()
                {
                    data = FlowModelList,
                    error = new Error(0, "读取成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 项目结题修改
        /// </summary>
        /// <param name="projectClosureModel"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public async Task<NewErrorModel> Modify([FromBody] ProjectClosureModel projectClosureModel)
        {
            try
            {
                if (string.IsNullOrEmpty(projectClosureModel.NodeId))
                {
                    return new NewErrorModel()
                    {
                        data = projectClosureModel,
                        error = new Error(1, "NodeId 没传！", "") { },
                    };
                }
                DDContext dDContext = new DDContext();
                Flows flows = dDContext.Flows.Where(f => f.FlowName.Contains("结题")).FirstOrDefault();
                ProjectClosure projectClosure = projectClosureModel.projectClosure;
                dDContext.Entry<ProjectClosure>(projectClosure).State = System.Data.Entity.EntityState.Modified;
                projectClosureModel.detailedLists.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                projectClosureModel.applicationUnitList.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });

                dDContext.ProjectFunding.AddRange(projectClosureModel.projectFundingList);

                projectClosureModel.longitudinalProject.ToList().ForEach(d =>
                {
                    dDContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                });
                List<NodeInfo> nodeInfos = dDContext.NodeInfo.Where(n => n.NodeName.Contains("财务负责人") && n.FlowId.ToString() == flows.FlowId.ToString()
                 && n.NodeId.ToString() == projectClosureModel.NodeId).ToList();
                if (nodeInfos.Count > 0)
                {
                    dDContext.ProjectFunding.AddRange(projectClosureModel.projectFundingList);
                }

                NodeInfo nodeInfo = dDContext.NodeInfo.Where(n => n.NodeName == "结束" && n.FlowId.ToString() == flows.FlowId.ToString()
                ).FirstOrDefault();


                //最后一步保存路径
                if (nodeInfo.NodeId == Int32.Parse(projectClosureModel.NodeId) + 1)
                {
                    Tasks tasks = dDContext.Tasks.Where(t => t.TaskId.ToString() == projectClosureModel.projectClosure.TaskId.ToString() && t.NodeId.ToString() == "0").FirstOrDefault();
                    ProjectInfo projectInfo = dDContext.ProjectInfo.Where(p => p.ProjectId == tasks.ProjectId.ToString()).FirstOrDefault();
                    projectInfo.ProjectState = "已完成";
                    dDContext.Entry<ProjectInfo>(projectInfo).State = System.Data.Entity.EntityState.Modified;
                    SavePath(projectInfo.FilePath + "1立项书或建议书", projectClosureModel.projectClosure.SuggestBook1);
                    SavePath(projectInfo.FilePath + "2评审PPT", projectClosureModel.projectClosure.PPT2);
                    SavePath(projectInfo.FilePath + "3需求规格说明书、产品总体设计书", projectClosureModel.projectClosure.DemandBook3);
                    SavePath(projectInfo.FilePath + "4机械设计图纸", projectClosureModel.projectClosure.Drawing4);
                    SavePath(projectInfo.FilePath + "5电气图纸", projectClosureModel.projectClosure.Electrical5);
                    SavePath(projectInfo.FilePath + "6BOM表", projectClosureModel.projectClosure.Bom6);
                    SavePath(projectInfo.FilePath + "7软件源代码", projectClosureModel.projectClosure.SourceCode7);
                    SavePath(projectInfo.FilePath + "8使用说明书、操作手册、技术方案、规格说明书", projectClosureModel.projectClosure.UseBook8);
                    SavePath(projectInfo.FilePath + "9合作协议", projectClosureModel.projectClosure.CooperationAgreement9);
                    SavePath(projectInfo.FilePath + "10产品（样机、成品）图片、影像", projectClosureModel.projectClosure.Product10);
                    SavePath(projectInfo.FilePath + "11阶段性整理的问题的分析、解决方案及计划表", projectClosureModel.projectClosure.Solution11);
                    //SavePath(projectInfo.FilePath + "12项目采购清单、借用清单、领料清单、入库清单", projectClosureModel.projectClosure);
                    //SavePath(projectInfo.FilePath + "13受理知识产权清单及申请资料", projectClosureModel.projectClosure);
                    SavePath(projectInfo.FilePath + "14纵向项目申请、中期检查、验收资料", projectClosureModel.projectClosure.AcceptanceData14);
                    SavePath(projectInfo.FilePath + "15其他过程文档", projectClosureModel.projectClosure.ProcessDocumentation15);
                    SavePath(projectInfo.FilePath + "16项目终止情况报告", projectClosureModel.projectClosure.TerminationReport16);
                    SavePath(projectInfo.FilePath + "17装箱单", projectClosureModel.projectClosure.PackingList17);
                    SavePath(projectInfo.FilePath + "18客户验收单", projectClosureModel.projectClosure.AcceptanceSlip18);
                    //SavePath(projectInfo.FilePath + "19转化、应用单位情况表", projectClosureModel.projectClosure.);
                    //SavePath(projectInfo.FilePath + "20项目经费使用情况表", projectClosureModel.projectClosure);

                    foreach (var item in projectClosureModel.detailedLists)
                    {
                        if (item.Type.Contains("零部件采购"))
                        {
                            Tasks taskP = dDContext.Tasks.Where(t => t.TaskId.ToString() == item.OldTaskId && t.NodeId == 0).FirstOrDefault();
                            PrintAndSendPurcahse(taskP, System.Web.HttpContext.Current.Server.MapPath
                           (projectInfo.FilePath + string.Format("12项目采购清单、借用清单、领料清单、入库清单//零部件采购单流水号{0}.zip", item.OldTaskId)));
                        }
                    }

                    List<LongitudinalProject> longitudinalProject = dDContext.LongitudinalProject.Where(p => p.TaskId == projectClosureModel.projectClosure.TaskId).ToList();

                    await PrintsLongitudinalProject(longitudinalProject, "", "纵向项目基本情况表模板",
                        "纵向项目基本情况表", 1, 2, System.Web.HttpContext.Current.Server.MapPath
                           (projectInfo.FilePath + "14纵向项目申请、中期检查、验收资料//纵向项目基本情况表.xlsx"));

                    List<ApplicationUnit> applicationUnitList = dDContext.ApplicationUnit.Where(p => p.TaskId == projectClosureModel.projectClosure.TaskId).ToList();
                    await PrintsApplicationUnit(applicationUnitList, "", "转化应用单位情况表模板",
                        "转化应用单位情况表", 1, 2, System.Web.HttpContext.Current.Server.MapPath
                           (projectInfo.FilePath + "19转化、应用单位情况表//转化应用单位情况表.xlsx"));

                    List<ProjectFunding> projectFundingList = dDContext.ProjectFunding.Where(p => p.TaskId == projectClosureModel.projectClosure.TaskId).ToList();
                    await PrintsProjectFunding(projectFundingList, "", "项目经费使用情况表模板",
                           "项目经费使用情况表", 0, 3, System.Web.HttpContext.Current.Server.MapPath
                           (projectInfo.FilePath + "20项目经费使用情况表//项目经费使用情况表.xlsx"));

                }

                dDContext.SaveChanges();
                return new NewErrorModel()
                {
                    data = projectClosureModel,
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 导出三张表格
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintExcel")]
        public async Task<object> PrintExcel([FromBody]PrintModelNew printAndSendModel)
        {
            try
            {
                if (printAndSendModel.Type == 0)
                {
                    return new NewErrorModel()
                    {
                        data = printAndSendModel,
                        error = new Error(1, "Type 没传！", "") { },
                    };
                }

                using (DDContext context = new DDContext())
                {
                    DataTable dtpurchaseTables = new DataTable();
                    switch (printAndSendModel.Type)
                    {
                        //纵向项目基本情况表
                        case 1:
                            List<LongitudinalProject> longitudinalProject = context.LongitudinalProject.Where(p => p.TaskId == printAndSendModel.TaskId).ToList();
                            return await PrintsLongitudinalProject(longitudinalProject, printAndSendModel.UserId, "纵向项目基本情况表模板",
                                "纵向项目基本情况表", 1, 2);
                        //转化应用单位情况表
                        case 2:
                            List<ApplicationUnit> applicationUnitList = context.ApplicationUnit.Where(p => p.TaskId == printAndSendModel.TaskId).ToList();
                            return await PrintsApplicationUnit(applicationUnitList, printAndSendModel.UserId, "转化应用单位情况表模板",
                                "转化应用单位情况表", 1, 2);
                        //项目经费使用情况表
                        case 3:
                            List<ProjectFunding> projectFundingList = context.ProjectFunding.Where(p => p.TaskId == printAndSendModel.TaskId).ToList();
                            return await PrintsProjectFunding(projectFundingList, printAndSendModel.UserId, "项目经费使用情况表模板",
                                   "项目经费使用情况表", 0, 3);
                    }
                }
                return new NewErrorModel()
                {
                    error = new Error(1, "Type 类型有误", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打印并推送表单
        /// </summary>
        /// <param name="printAndSendModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintAndSend")]
        public async Task<NewErrorModel> PrintAndSend([FromBody]PrintAndSendModel printAndSendModel)
        {
            try
            {
                string TaskId = printAndSendModel.TaskId;
                string UserId = printAndSendModel.UserId;
                PDFHelper pdfHelper = new PDFHelper();
                using (DDContext context = new DDContext())
                {
                    //获取表单信息
                    Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.NodeId == 0).First();
                    string FlowId = tasks.FlowId.ToString();
                    string ProjectId = tasks.ProjectId;
                    //判断流程是否已结束

                    List<Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == TaskId && t.State == 0 && t.IsSend == false).ToList();
                    if (tasksList.Count > 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "流程未结束！", "") { },
                        };
                    }
                    
                    ProjectClosure purchaseTableList = context.ProjectClosure.Where(u => u.TaskId == TaskId).FirstOrDefault();
                    Dictionary<string, string> keyValuePairsHead = new Dictionary<string, string>();
                    keyValuePairsHead.Add("项目类别", purchaseTableList.ProjectType);
                    keyValuePairsHead.Add("项目负责人", purchaseTableList.ResponsibleMan);
                    keyValuePairsHead.Add("小组成员", purchaseTableList.TeamMembers);
                    keyValuePairsHead.Add("项目周期", purchaseTableList.StartTime + "-" + purchaseTableList.EndTime);
                    keyValuePairsHead.Add("项目实际开发周期", purchaseTableList.ActualCycleStart + "-" + purchaseTableList.ActualCycleEnd);
                    keyValuePairsHead.Add("是否有横向合作单位", purchaseTableList.IsTransverse == true ? "是" : "否");
                    if (purchaseTableList.IsTransverse == true)
                    {
                        keyValuePairsHead.Add("合同金额", purchaseTableList.ContractAmount);
                        keyValuePairsHead.Add("合同编码", purchaseTableList.ContractNo);
                        keyValuePairsHead.Add("实际到账", purchaseTableList.ActualMoney);
                    }
                    keyValuePairsHead.Add("是否有申报纵向项目", purchaseTableList.IsPortrait == true ? "是" : "否");

                    List<PrintPDFModel> printPDFModels = new List<PrintPDFModel>();
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "立项书或建议书",
                        Count = CalCounts(purchaseTableList.SuggestBook1).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "评审PPT",
                        Count = CalCounts(purchaseTableList.PPT2).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "需求规格说明书、产品总体设计书",
                        Count = CalCounts(purchaseTableList.DemandBook3).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "机械设计图纸",
                        Count = CalCounts(purchaseTableList.Drawing4).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "电气图纸",
                        Count = CalCounts(purchaseTableList.Electrical5).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "BOM表",
                        Count = CalCounts(purchaseTableList.Bom6).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "软件源代码",
                        Count = CalCounts(purchaseTableList.SourceCode7).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "使用说明书/操作手册/技术方案/规格说明书",
                        Count = CalCounts(purchaseTableList.UseBook8).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "合作协议",
                        Count = CalCounts(purchaseTableList.CooperationAgreement9).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "产品（样机/成品）图片/影像",
                        Count = CalCounts(purchaseTableList.Product10).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "阶段性整理的问题的分析、解决方案及计划表",
                        Count = CalCounts(purchaseTableList.Solution11).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "项目采购清单、借用清单、领料清单、入库清单、借用清单、维修清单",
                        Count = context.DetailedList.Where(t => t.TaskId == TaskId && t.Type.Contains("零部件采购")).ToList().Count.ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "受理知识产权清单",
                        Count = context.DetailedList.Where(t => t.TaskId == TaskId && t.Type.Contains("知识产权")).ToList().Count.ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "纵向项目申请/中期检查/验收资料",
                        Count = CalCounts(purchaseTableList.AcceptanceData14).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "其他过程文档、设计报告、评审报告、项目计划、设计更改报告等",
                        Count = CalCounts(purchaseTableList.ProcessDocumentation15).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "项目终止情况报告",
                        Count = CalCounts(purchaseTableList.TerminationReport16).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "装箱单",
                        Count = CalCounts(purchaseTableList.PackingList17).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "客户验收单",
                        Count = CalCounts(purchaseTableList.AcceptanceSlip18).ToString()
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "转化/应用单位情况表",
                        Count = "1"
                    });
                    printPDFModels.Add(new PrintPDFModel()
                    {
                        Name = "项目经费使用情况表",
                        Count = "1"
                    });

                    DataTable dtSourse = DtLinqOperators.CopyToDataTable(printPDFModels);
                    //ClassChangeHelper.ToDataTable(SelectPurchaseList);
                    List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == FlowId && u.NodeId != 0 && u.IsSend != true && u.NodeName != "结束").ToList();
                    foreach (NodeInfo nodeInfo in NodeInfoList)
                    {
                        if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                        {
                            string strNodePeople = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyMan;
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = strNodePeople + "  " + ApplyTime;
                        }
                        else
                        {
                            string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == TaskId && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                            nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + ApplyTime;
                        }
                    }
                    DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                    string FlowName = context.Flows.Where(f => f.FlowId.ToString() == FlowId).First().FlowName.ToString();
                    ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == ProjectId).First();
                    string ProjectName = projectInfo.ProjectName;
                    string ProjectNo = projectInfo.ProjectId;

                    //绘制BOM表单PDF
                    List<string> contentList = new List<string>()
                        {
                            "序号","文件类别", "份数"
                        };

                    float[] contentWithList = new float[]
                    {
                        50, 350, 60,
                    };

                    string path = pdfHelper.GeneratePDF(FlowName, TaskId, tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                    ProjectName, ProjectNo, "2", 300, 650, contentList, contentWithList, dtSourse, dtApproveView, null, keyValuePairsHead);
                    string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);

                    List<string> newPaths = new List<string>();
                    RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                    newPaths.Add(RelativePath);
                    string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, FlowName + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //文件压缩打包
                    IonicHelper.CompressMulti(newPaths, SavePath, false);

                    //上传盯盘获取MediaId
                    SavePath = string.Format(@"~\UploadFile\Ionic\{0}", Path.GetFileName(SavePath));
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    var resultUploadMedia = await dingTalkServersController.UploadMedia(SavePath);
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = UserId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);

                    return new NewErrorModel()
                    {
                        error = new Error(0, result, "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 计算文件数
        /// </summary>
        /// <param name="strObj"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CalCounts")]
        public int CalCounts(string strObj)
        {
            if (string.IsNullOrEmpty(strObj))
            {
                return 0;
            }
            else
            {
                List<FileUrlModel> fileUrlModels = JsonConvert.DeserializeObject<List<FileUrlModel>>(strObj);
                return fileUrlModels.Count;
            }
        }

        /// <summary>
        /// 后端打印用(采购表单)
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="strpath"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PrintAndSendPurcahse")]
        public NewErrorModel PrintAndSendPurcahse(Tasks tasks, string strpath)
        {
            try
            {
                DDContext context = new DDContext();
                List<PurchaseTable> PurchaseTableList = context.PurchaseTable.Where(u => u.TaskId == tasks.TaskId.ToString()).ToList();

                var SelectPurchaseList = from p in PurchaseTableList
                                         select new
                                         {
                                             p.CodeNo,
                                             p.Name,
                                             p.Standard,
                                             p.Unit,
                                             p.Count,
                                             p.Price,
                                             p.Purpose,
                                             p.UrgentDate,
                                             p.Mark
                                         };
                DataTable dtSourse = DtLinqOperators.CopyToDataTable(SelectPurchaseList);
                //ClassChangeHelper.ToDataTable(SelectPurchaseList);
                List<NodeInfo> NodeInfoList = context.NodeInfo.Where(u => u.FlowId == tasks.FlowId.ToString() && u.NodeId != 0 && u.IsSend != true && u.NodeName != "结束").ToList();
                foreach (NodeInfo nodeInfo in NodeInfoList)
                {
                    if (string.IsNullOrEmpty(nodeInfo.NodePeople))
                    {
                        string strNodePeople = context.Tasks.Where(q => q.TaskId.ToString() == tasks.TaskId.ToString() && q.NodeId == nodeInfo.NodeId).First().ApplyMan;
                        string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == tasks.TaskId.ToString() && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                        nodeInfo.NodePeople = strNodePeople + "  " + ApplyTime;
                    }
                    else
                    {
                        string ApplyTime = context.Tasks.Where(q => q.TaskId.ToString() == tasks.TaskId.ToString() && q.NodeId == nodeInfo.NodeId).First().ApplyTime;
                        nodeInfo.NodePeople = nodeInfo.NodePeople + "  " + ApplyTime;
                    }
                }
                DataTable dtApproveView = ClassChangeHelper.ToDataTable(NodeInfoList);
                string FlowName = context.Flows.Where(f => f.FlowId.ToString() == tasks.FlowId.ToString()).First().FlowName.ToString();
                ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == tasks.ProjectId).First();
                string ProjectName = projectInfo.ProjectName;
                string ProjectNo = projectInfo.ProjectId;

                //绘制BOM表单PDF
                List<string> contentList = new List<string>()
                        {
                            "序号","物料编码","物料名称","规格型号","单位","数量","单价","用途","需用日期","备注"
                        };

                float[] contentWithList = new float[]
                {
                        50, 60, 60, 60, 60, 60, 60, 60, 60,60
                };
                PDFHelper pdfHelper = new PDFHelper();
                string path = pdfHelper.GeneratePDF(FlowName, tasks.TaskId.ToString(), tasks.ApplyMan, tasks.Dept, tasks.ApplyTime,
                ProjectName, ProjectNo, "2", 300, 650, contentList, contentWithList, dtSourse, dtApproveView, null);
                string RelativePath = "~/UploadFile/PDF/" + Path.GetFileName(path);

                List<string> newPaths = new List<string>();
                RelativePath = AppDomain.CurrentDomain.BaseDirectory + RelativePath.Substring(2, RelativePath.Length - 2).Replace('/', '\\');
                newPaths.Add(RelativePath);
                //string SavePath = string.Format(@"{0}\UploadFile\Ionic\{1}.zip", AppDomain.CurrentDomain.BaseDirectory, FlowName + DateTime.Now.ToString("yyyyMMddHHmmss"));
                //文件压缩打包
                IonicHelper.CompressMulti(newPaths, strpath, false);


                return new NewErrorModel()
                {
                    error = new Error(0, "", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NewErrorModel> PrintsLongitudinalProject(List<LongitudinalProject> items, string userId, string templetName, string fileName, int column,
            int row, string copyPath = "", string sheetName = "Sheet1")
        {
            if (items == null)
            {
                return new NewErrorModel()
                {
                    error = new Error(0, "暂无数据", "") { },
                };
            }
            DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(items, new List<string>() {
                "Id","TaskId"
            });
            string path = HttpContext.Current.Server.MapPath(string.Format("~/UploadFile/Excel/Templet/{0}.xlsx", templetName));
            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\" + fileName + time + ".xlsx";
            File.Copy(path, newPath, true);
            if (copyPath == "")
            {
                if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row))
                {
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    //上盯盘
                    var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/" + fileName + time + ".xlsx");
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = userId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                    //删除文件
                    File.Delete(newPath);
                    return new NewErrorModel()
                    {
                        error = new Error(0, result, "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "文件有误", "") { },
                    };
                }
            }
            else
            {
                ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row);
                string pathTest = Path.GetDirectoryName(copyPath);
                if (!Directory.Exists(pathTest))
                {
                    Directory.CreateDirectory(pathTest);
                }
                File.Copy(newPath, copyPath, true);
                return new NewErrorModel()
                {
                    error = new Error(0, "复制成功！", "") { },
                };
            }
        }
        public async Task<NewErrorModel> PrintsApplicationUnit(List<ApplicationUnit> items, string userId, string templetName, string fileName, int column,
         int row, string copyPath = "", string sheetName = "Sheet1")
        {
            if (items == null)
            {
                return new NewErrorModel()
                {
                    error = new Error(0, "暂无数据", "") { },
                };
            }
            DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(items, new List<string>() {
                "Id","TaskId"
            });
            string path = HttpContext.Current.Server.MapPath(string.Format("~/UploadFile/Excel/Templet/{0}.xlsx", templetName));
            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\" + fileName + time + ".xlsx";
            File.Copy(path, newPath);
            if (copyPath == "")
            {
                if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row))
                {
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    //上盯盘
                    var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/" + fileName + time + ".xlsx");
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = userId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                    //删除文件
                    File.Delete(newPath);
                    return new NewErrorModel()
                    {
                        error = new Error(0, result, "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "文件有误", "") { },
                    };
                }
            }
            else
            {
                ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row);
                string pathTest = Path.GetDirectoryName(copyPath);
                if (!Directory.Exists(pathTest))
                {
                    Directory.CreateDirectory(pathTest);
                }
                File.Copy(newPath, copyPath, true);
                return new NewErrorModel()
                {
                    error = new Error(0, "复制成功！", "") { },
                };
            }
        }


        public async Task<NewErrorModel> PrintsProjectFunding(List<ProjectFunding> items, string userId, string templetName, string fileName, int column,
       int row, string copyPath = "", string sheetName = "Sheet1")
        {
            if (items == null)
            {
                return new NewErrorModel()
                {
                    error = new Error(0, "暂无数据", "") { },
                };
            }
            DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(items, new List<string>() {
                "Id","TaskId"
            });
            string path = HttpContext.Current.Server.MapPath(string.Format("~/UploadFile/Excel/Templet/{0}.xlsx", templetName));
            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\" + fileName + time + ".xlsx";
            File.Copy(path, newPath);

            if (copyPath == "")
            {
                if (ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row))
                {
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    //上盯盘
                    var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/" + fileName + time + ".xlsx");
                    //推送用户
                    FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                    fileSendModel.UserId = userId;
                    var result = await dingTalkServersController.SendFileMessage(fileSendModel);
                    //删除文件
                    File.Delete(newPath);
                    return new NewErrorModel()
                    {
                        error = new Error(0, result, "") { },
                    };
                }
                else
                {
                    return new NewErrorModel()
                    {
                        error = new Error(1, "文件有误", "") { },
                    };
                }
            }
            else
            {
                ExcelHelperByNPOI.UpdateExcel(newPath, "Sheet1", dtpurchaseTables, column, row);
                string pathTest = Path.GetDirectoryName(copyPath);
                if (!Directory.Exists(pathTest))
                {
                    Directory.CreateDirectory(pathTest);
                }
                File.Copy(newPath, copyPath, true);
                return new NewErrorModel()
                {
                    error = new Error(0, "复制成功！", "") { },
                };
            }
        }

        public void SavePath(string path, string strObject)
        {
            if (!string.IsNullOrEmpty(strObject))
            {
                List<FlieUrlModel> flieUrlModels = JsonConvert.DeserializeObject<List<FlieUrlModel>>(strObject);
                if (flieUrlModels.Count > 0 && !string.IsNullOrEmpty(path))
                {
                    foreach (var url in flieUrlModels)
                    {
                        string filename = System.IO.Path.GetFileName(System.Web.HttpContext.Current.Server.MapPath(url.FileUrl));

                        //判断文件是否存在
                        if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(url.FileUrl)))
                        {
                            string copypath = System.Web.HttpContext.Current.Server.MapPath("~" + path) + "\\" + filename;
                            string pathNew = Path.GetDirectoryName(copypath);
                            if (!Directory.Exists(pathNew))
                            {
                                Directory.CreateDirectory(pathNew);
                            }
                            File.Copy(System.Web.HttpContext.Current.Server.MapPath(url.FileUrl), copypath,true);
                            //FileHelper.Copy(System.Web.HttpContext.Current.Server.MapPath(url.FileUrl), copypath);
                        }
                        //File.Copy(System.Web.HttpContext.Current.Server.MapPath(url.FileUrl),
                        //    System.Web.HttpContext.Current.Server.MapPath("~" + path));
                    }
                }
            }
        }
    }

    public class PrintModelNew
    {
        /// <summary>
        /// 推送用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 打印类型 1 表示 纵向项目基本情况表 2 表示转化/应用单位情况表 3 项目经费使用情况表（实际支出）
        /// </summary>
        public int Type { get; set; }
    }

    public class FlieUrlModel
    {
        public string FileUrl { get; set; }
        public string MediaId { get; set; }
        public string FileName { get; set; }
    }

    public class PrintPDFModel
    {
        public string Name { get; set; }
        public string Count { get; set; }
    }

    public class FlowModel
    {
        public string Type { get; set; }
        public string taskId { get; set; }
        public string ApplyMan { get; set; }
        public string ApplyManId { get; set; }
        public string ApplyTime { get; set; }
        /// <summary>
        /// 其他流程的TaskId
        /// </summary>
        public string OldtaskId { get; set; }
    }

    public class FileUrlModel
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string fileUrl { get; set; }
        /// <summary>
        /// 盯盘Id
        /// </summary>
        public string mediaId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
    }

    public class ProjectClosureModel
    {
        /// <summary>
        /// 当前NodeId 用于判断流程是否结束
        /// </summary>
        public string NodeId { get; set; }
        public ProjectClosure projectClosure { get; set; }

        /// <summary>
        /// 项目采购清单、借用清单、领料清单、入库清单、借用清单、维修清单、受理知识产权清单
        /// </summary>
        public List<DetailedList> detailedLists { get; set; }

        /// <summary>
        /// 转化/应用单位
        /// </summary>
        public List<ApplicationUnit> applicationUnitList { get; set; }

        /// <summary>
        /// 项目经费使用情况
        /// </summary>
        public List<ProjectFunding> projectFundingList { get; set; }

        /// <summary>
        /// 纵向项目基本情况表
        /// </summary>
        public List<LongitudinalProject> longitudinalProject { get; set; }

    }
}
