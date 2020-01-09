using Common.ClassChange;
using Common.Excel;
using DingTalk.EF;
using DingTalk.Models;
using DingTalk.Models.DingModels;
using DingTalk.Models.DingModelsHs;
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
    /// 生产加工进度表
    /// </summary>
    [RoutePrefix("ProcessingProgress")]
    public class ProcessingProgressController : ApiController
    {
        /// <summary>
        /// 生产加工进度表批量保存
        /// </summary>
        /// <param name="processingProgressModel"></param>
        /// <returns></returns>
        [Route("Save")]
        [HttpPost]
        public async Task<NewErrorModel> Save(ProcessingProgressModel processingProgressModel)
        {
            try
            {
                if (processingProgressModel.CompanyId == 0)  //研究院
                {
                    DDContext dDContext = new DDContext();
                    string eappUrl = string.Format("eapp://page/start/productionMonitoring/productionMonitoring?taskid={0}&companyId={1}", processingProgressModel.processingProgresses[0].TaskId, processingProgressModel.CompanyId);
                    if (dDContext.Roles.Where(r => r.RoleName == "生产加工进度发起人" && r.UserId == processingProgressModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限上传！", "") { },
                        };
                    }

                    List<Models.DingModels.ProjectInfo> projectInfos = dDContext.ProjectInfo.ToList();

                    if (processingProgressModel.IsExcelUpload)
                    {
                        foreach (var processingProgresse in processingProgressModel.processingProgresses)
                        {
                            //校对数据
                            if (!string.IsNullOrEmpty(processingProgresse.TaskId))
                            {
                                processingProgresse.CompanyId = processingProgressModel.CompanyId.ToString();
                                List<ProcessingProgress> ProcessingProgressList = dDContext.ProcessingProgress.Where(p => p.TaskId == processingProgresse.TaskId && p.CompanyId == processingProgressModel.CompanyId.ToString()).ToList();
                                if (ProcessingProgressList.Count > 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("保存失败,系统中已存在流水号 {0} 的数据", processingProgresse.TaskId), "") { },
                                    };
                                }
                                if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                                 p.ProjectName == processingProgresse.ProjectName).ToList().Count == 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的数据不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                    };
                                }
                                else
                                {
                                    if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                                  p.ProjectName == p.ProjectName && p.ProjectType == processingProgresse.ProjectType
                                  && p.ProjectSmallType == processingProgresse.ProjectSmallType).ToList().Count == 0)
                                    {
                                        return new NewErrorModel()
                                        {
                                            error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的大类、小类不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                        };
                                    }
                                    else
                                    {
                                        List<Models.DingModels.Tasks> tasksDesigner = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Designer)).ToList();
                                        List<Models.DingModels.Tasks> tasksNoteTaker = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.NoteTaker)).ToList();
                                        List<Models.DingModels.Tasks> tasksHeadOfDepartments = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.HeadOfDepartments)).ToList();
                                        List<Models.DingModels.Tasks> tasksTabulator = dDContext.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Tabulator)).ToList();
                                        if (tasksDesigner.Count == 0 || tasksNoteTaker.Count == 0 || tasksHeadOfDepartments.Count == 0 || tasksTabulator.Count == 0)
                                        {
                                            if (tasksTabulator.Count == 0)
                                            {
                                                return new NewErrorModel()
                                                {
                                                    error = new Error(1, string.Format("保存失败,系统中找不到：制表人 {0} 的Id   ！", processingProgresse.Tabulator), "") { },
                                                };
                                            }
                                            if (tasksDesigner.Count == 0)
                                            {
                                                return new NewErrorModel()
                                                {
                                                    error = new Error(1, string.Format("保存失败,系统中找不到：设计员 {0} 的Id   ！", processingProgresse.NoteTaker), "") { },
                                                };
                                            }
                                            if (tasksNoteTaker.Count == 0)
                                            {
                                                return new NewErrorModel()
                                                {
                                                    error = new Error(1, string.Format("保存失败,系统中找不到：记录员 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                                };
                                            }
                                            if (tasksHeadOfDepartments.Count == 0)
                                            {
                                                return new NewErrorModel()
                                                {
                                                    error = new Error(1, string.Format("保存失败,系统中找不到：部门负责人 {0} 的Id   ！", processingProgresse.HeadOfDepartments), "") { },
                                                };
                                            }
                                        }
                                        else
                                        {
                                            processingProgresse.TabulatorId = tasksTabulator[0].ApplyManId;
                                            processingProgresse.DesignerId = tasksDesigner[0].ApplyManId;
                                            processingProgresse.NoteTakerId = tasksNoteTaker[0].ApplyManId;
                                            processingProgresse.HeadOfDepartmentsId = tasksHeadOfDepartments[0].ApplyManId;
                                            processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                                            dDContext.ProcessingProgress.Add(processingProgresse);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!processingProgressModel.IsExcelUpload)  //操作界面添加
                    {
                        List<ProcessingProgress> ProcessingProgressList = new List<ProcessingProgress>();

                        foreach (ProcessingProgress processingProgresse in processingProgressModel.processingProgresses)
                        {
                            List<ProcessingProgress> ProcessingProgressListNew = dDContext.ProcessingProgress.Where(p => p.TaskId == processingProgresse.TaskId && p.CompanyId == processingProgressModel.CompanyId.ToString()).ToList();
                            if (ProcessingProgressListNew.Count > 0)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, string.Format("保存失败,系统中已存在流水号 {0} 的数据", processingProgresse.TaskId), "") { },
                                };
                            }
                            processingProgresse.CompanyId = processingProgressModel.CompanyId.ToString();
                            Roles roles = dDContext.Roles.Where(r => r.RoleName == "生产加工进度分配人").FirstOrDefault();
                            //推送钉钉消息给设计人员和部门负责人(胡工)
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            await dingTalkServersController.SendProcessingProgress(processingProgresse.DesignerId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                                , processingProgresse.TaskId, processingProgresse.CompanyName, processingProgresse.SpeedOfProgress, processingProgresse.IsAlreadyRead, eappUrl);

                            await dingTalkServersController.SendProcessingProgress(roles.UserId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                              , processingProgresse.TaskId, processingProgresse.CompanyName, processingProgresse.SpeedOfProgress, processingProgresse.IsAlreadyRead, eappUrl);

                            processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                            ProcessingProgressList.Add(processingProgresse);
                        }
                        dDContext.ProcessingProgress.AddRange(ProcessingProgressList);
                    }
                    dDContext.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, string.Format("保存成功！共计{0}条数据 ", processingProgressModel.processingProgresses.Count), "") { },
                    };
                }
                else  //华数
                {
                    DDContext dDContext = new DDContext();
                    DDContextHs dDContextHs = new DDContextHs();
                    string eappUrl = string.Format("eapp://page/start/productionMonitoring/productionMonitoring?taskid={0}&companyId={1}", processingProgressModel.processingProgresses[0].TaskId, processingProgressModel.CompanyId);
                    if (dDContext.Roles.Where(r => r.RoleName == "生产加工进度发起人" && r.UserId == processingProgressModel.applyManId).ToList().Count == 0)
                    {
                        return new NewErrorModel()
                        {
                            error = new Error(1, "没有权限上传！", "") { },
                        };
                    }

                    List<Models.DingModelsHs.ProjectInfo> projectInfos = dDContextHs.ProjectInfo.ToList();
                    if (processingProgressModel.IsExcelUpload)  //操作界面添加
                    {
                        foreach (var processingProgresse in processingProgressModel.processingProgresses)
                        {
                            //校对数据
                            if (!string.IsNullOrEmpty(processingProgresse.TaskId))
                            {
                                processingProgresse.CompanyId = processingProgressModel.CompanyId.ToString();
                                List<ProcessingProgress> ProcessingProgressList = dDContext.ProcessingProgress.Where(p => p.TaskId == processingProgresse.TaskId).ToList();
                                if (ProcessingProgressList.Count > 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("保存失败,系统中已存在流水号 {0} 的数据", processingProgresse.TaskId), "") { },
                                    };
                                }
                                if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                                 p.ProjectName == processingProgresse.ProjectName).ToList().Count == 0)
                                {
                                    return new NewErrorModel()
                                    {
                                        error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的数据不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                    };
                                }
                                else
                                {
                                    //华数不校对项目类别
                                    //  if (projectInfos.Where(p => p.ProjectId == processingProgresse.ProjectId &&
                                    //p.ProjectName == p.ProjectName && p.ProjectType == processingProgresse.ProjectType
                                    //&& p.ProjectSmallType == processingProgresse.ProjectSmallType).ToList().Count == 0)
                                    //  {
                                    //      return new NewErrorModel()
                                    //      {
                                    //          error = new Error(1, string.Format("保存失败,项目Id {0} 、 项目名 {1} 与系统中的大类、小类不吻合！", processingProgresse.ProjectId, processingProgresse.ProjectName), "") { },
                                    //      };
                                    //  }
                                    //  else
                                    //  {
                                    List<Models.DingModelsHs.Tasks> tasksDesigner = dDContextHs.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Designer)).ToList();
                                    List<Models.DingModelsHs.Tasks> tasksNoteTaker = dDContextHs.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.NoteTaker)).ToList();
                                    List<Models.DingModelsHs.Tasks> tasksHeadOfDepartments = dDContextHs.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.HeadOfDepartments)).ToList();
                                    List<Models.DingModelsHs.Tasks> tasksTabulator = dDContextHs.Tasks.Where(t => t.ApplyMan.Contains(processingProgresse.Tabulator)).ToList();
                                    if (tasksDesigner.Count == 0 || tasksNoteTaker.Count == 0 || tasksHeadOfDepartments.Count == 0 || tasksTabulator.Count == 0)
                                    {
                                        if (tasksTabulator.Count == 0)
                                        {
                                            return new NewErrorModel()
                                            {
                                                error = new Error(1, string.Format("保存失败,系统中找不到：制表人 {0} 的Id   ！", processingProgresse.Tabulator), "") { },
                                            };
                                        }
                                        if (tasksDesigner.Count == 0)
                                        {
                                            return new NewErrorModel()
                                            {
                                                error = new Error(1, string.Format("保存失败,系统中找不到：设计员 {0} 的Id   ！", processingProgresse.NoteTaker), "") { },
                                            };
                                        }
                                        if (tasksNoteTaker.Count == 0)
                                        {
                                            return new NewErrorModel()
                                            {
                                                error = new Error(1, string.Format("保存失败,系统中找不到：记录员 {0} 的Id   ！", processingProgresse.Designer), "") { },
                                            };
                                        }
                                        if (tasksHeadOfDepartments.Count == 0)
                                        {
                                            return new NewErrorModel()
                                            {
                                                error = new Error(1, string.Format("保存失败,系统中找不到：部门负责人 {0} 的Id   ！", processingProgresse.HeadOfDepartments), "") { },
                                            };
                                        }
                                    }
                                    else
                                    {
                                        processingProgresse.TabulatorId = tasksTabulator[0].ApplyManId;
                                        processingProgresse.DesignerId = tasksDesigner[0].ApplyManId;
                                        processingProgresse.NoteTakerId = tasksNoteTaker[0].ApplyManId;
                                        processingProgresse.HeadOfDepartmentsId = tasksHeadOfDepartments[0].ApplyManId;
                                        processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                                        dDContext.ProcessingProgress.Add(processingProgresse);
                                    }
                                }
                            }
                        }
                    }
                    if (!processingProgressModel.IsExcelUpload)  //操作界面添加
                    {
                        List<ProcessingProgress> ProcessingProgressList = new List<ProcessingProgress>();
                        foreach (var processingProgresse in processingProgressModel.processingProgresses)
                        {
                            List<ProcessingProgress> ProcessingProgressListNew = dDContext.ProcessingProgress.Where(p => p.TaskId == processingProgresse.TaskId && p.CompanyId == processingProgressModel.CompanyId.ToString()).ToList();
                            if (ProcessingProgressListNew.Count > 0)
                            {
                                return new NewErrorModel()
                                {
                                    error = new Error(1, string.Format("保存失败,系统中已存在流水号 {0} 的数据", processingProgresse.TaskId), "") { },
                                };
                            }
                            processingProgresse.CompanyId = processingProgressModel.CompanyId.ToString();
                            Roles roles = dDContext.Roles.Where(r => r.RoleName == "生产加工进度分配人").FirstOrDefault();
                            //推送钉钉消息给设计人员和部门负责人(胡工)
                            DingTalkServersController dingTalkServersController = new DingTalkServersController();
                            await dingTalkServersController.SendProcessingProgress(processingProgresse.DesignerId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                                , processingProgresse.TaskId, processingProgresse.CompanyName, processingProgresse.SpeedOfProgress, processingProgresse.IsAlreadyRead, eappUrl);

                            await dingTalkServersController.SendProcessingProgress(roles.UserId, 0, processingProgressModel.applyMan, processingProgresse.Bom
                              , processingProgresse.TaskId, processingProgresse.CompanyName, processingProgresse.SpeedOfProgress, processingProgresse.IsAlreadyRead, eappUrl);

                            processingProgresse.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                            ProcessingProgressList.Add(processingProgresse);
                        }
                        dDContext.ProcessingProgress.AddRange(ProcessingProgressList);
                    }
                    dDContext.SaveChanges();
                    return new NewErrorModel()
                    {
                        error = new Error(0, string.Format("保存成功！共计{0}条数据 ", processingProgressModel.processingProgresses.Count), "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 生产加工进度表批量读取
        /// </summary>
        /// <param name="applyManId">查询人Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="projectType">项目大类</param>
        /// <param name="projectSmallType">小类</param>
        /// <param name="taskId">流水号</param>
        /// <param name="key">关键字(项目名、BOM、设计员、记录人)</param>
        /// <param name="IsPrint">是否推送Excel</param>
        /// <param name="companyId">公司Id 0 研究院 1 华数 （不传默认两家公司）</param>
        /// <returns></returns>
        [Route("Read")]
        [HttpGet]
        public async Task<NewErrorModel> Read(string applyManId, int pageIndex, int pageSize, string projectType = "",
            string projectSmallType = "", string taskId = "", string key = "", bool IsPrint = false, int companyId = 3)
        {
            try
            {
                DDContext context = new DDContext();
                List<ProcessingProgress> processingProgresses =
                    context.ProcessingProgress.Where(t => t.TabulatorId.Contains(applyManId) ||
               t.DesignerId.Contains(applyManId) || t.HeadOfDepartmentsId.Contains(applyManId)
               || t.NoteTakerId.Contains(applyManId)).ToList();
                if (companyId != 3)
                {
                    processingProgresses = processingProgresses.Where(p => p.CompanyId == companyId.ToString()).ToList();
                }
                processingProgresses = processingProgresses.Where(t =>
               (taskId != "" ? t.TaskId == taskId : 1 == 1)).ToList();
                processingProgresses = processingProgresses.Where(t =>
               (key != "" ? (t.ProjectName.Contains(key) || (t.Bom.Contains(key) || (t.Designer.Contains(key) || (t.NoteTaker.Contains(key))))) : 1 == 1)).ToList();

                processingProgresses = processingProgresses.Where(t =>
             (projectType != "" ? t.ProjectType == projectType : 1 == 1)
             && (projectSmallType != "" ? t.ProjectSmallType == projectSmallType : 1 == 1)).OrderBy(t => t.SpeedOfProgress).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                foreach (var item in processingProgresses)
                {
                    NewErrorModel errorModel = GetPower(applyManId, item.TaskId);
                    item.Power = (List<int>)errorModel.data;
                }
                if (IsPrint == false)
                {
                    return new NewErrorModel()
                    {
                        count = processingProgresses.Count,
                        data = processingProgresses,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    DataTable dtpurchaseTables = ClassChangeHelper.ToDataTable(processingProgresses, new List<string>() {
                          "DesignerId","CompanyId","HeadOfDepartmentsId","NoteTakerId","TabulatorId","CreateTime","FinishTime","Power"
                         });
                    string path = HttpContext.Current.Server.MapPath(string.Format("~/UploadFile/Excel/Templet/{0}.xlsx", "生产加工进度表模板"));
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newPath = HttpContext.Current.Server.MapPath("~/UploadFile/Excel/Templet") + "\\" + "生产加工进度表" + time + ".xlsx";
                    File.Copy(path, newPath, true);
                    if (ExcelHelperByNPOI.UpdateExcel(newPath, "研究院+华数", dtpurchaseTables, 0, 3))
                    {
                        DingTalkServersController dingTalkServersController = new DingTalkServersController();
                        //上盯盘
                        var resultUploadMedia = await dingTalkServersController.UploadMedia("~/UploadFile/Excel/Templet/" + "生产加工进度表" + time + ".xlsx");
                        //推送用户
                        FileSendModel fileSendModel = JsonConvert.DeserializeObject<FileSendModel>(resultUploadMedia);
                        fileSendModel.UserId = applyManId;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="processingProgressModel"></param>
        /// <returns></returns>
        [Route("Modify")]
        [HttpPost]
        public async Task<NewErrorModel> Modify(ProcessingProgressModel processingProgressModel)
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    DingTalkServersController dingTalkServersController = new DingTalkServersController();
                    foreach (var item in processingProgressModel.processingProgresses)
                    {
                        string eappUrl = string.Format("eapp://page/start/productionMonitoring/productionMonitoring?taskid={0}&companyId={1}", item.TaskId, item.CompanyId);
                        //判断当前修改权限
                        NewErrorModel errorModel = GetPower(processingProgressModel.applyManId, item.TaskId);
                        List<int> vs = (List<int>)errorModel.data;

                        if (vs.Contains(1) && vs.Contains(3)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送制表人
                                await dingTalkServersController.SendProcessingProgress(item.TabulatorId, 2, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                                //推送设计人员
                                await dingTalkServersController.SendProcessingProgress(item.DesignerId, 2, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                            }
                            context.SaveChanges();
                            return new NewErrorModel()
                            {
                                error = new Error(0, "修改成功！", "") { },
                            };
                        }

                        if (vs.Count == 1 && vs.Contains(1)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送实际记录人
                                await dingTalkServersController.SendProcessingProgress(item.NoteTakerId, 3, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                            }
                        }
                        if (vs.Count == 1 && vs.Contains(0) ) //制表人 暂时不通知(添加的时候通知了)
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        if (vs.Count == 1 && vs.Contains(2)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            if (item.IsAlreadyRead == true)
                            {
                                item.FinishTime = DateTime.Now.ToString("yyyy-MM-dd");
                            }
                            //修改已读状态
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;

                            //推送制表人
                            await dingTalkServersController.SendProcessingProgress(item.TabulatorId, 1, processingProgressModel.applyMan, item.Bom
                                , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                            //推送分配人
                            await dingTalkServersController.SendProcessingProgress(item.HeadOfDepartmentsId, 1, processingProgressModel.applyMan, item.Bom
                                , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                        }
                        if (/*vs.Count == 1 &&*/ vs.Contains(3)) //  0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人
                        {
                            context.Entry<ProcessingProgress>(item).State = System.Data.Entity.EntityState.Modified;
                            if (!string.IsNullOrEmpty(item.SpeedOfProgress)) //获取工作进度表状态
                            {
                                //推送制表人
                                await dingTalkServersController.SendProcessingProgress(item.TabulatorId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                                //推送设计人员
                                await dingTalkServersController.SendProcessingProgress(item.DesignerId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                                //推送分配人
                                await dingTalkServersController.SendProcessingProgress(item.HeadOfDepartmentsId, 0, processingProgressModel.applyMan, item.Bom
                                    , item.TaskId, item.CompanyName, item.SpeedOfProgress, item.IsAlreadyRead, eappUrl);
                            }
                        }
                    }
                    context.SaveChanges();
                }
                return new NewErrorModel()
                {
                    error = new Error(0, "修改成功！", "") { },
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图纸审批默认数据读取
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="CompanyId">公司Id 0 研究院 1 华数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DefaultRead")]
        public NewErrorModel DefaultRead(string taskId, int CompanyId = 0)
        {
            try
            {
                if (CompanyId == 0)  //研究院
                {
                    ProcessingProgress processingProgress = new ProcessingProgress();
                    using (DDContext context = new DDContext())
                    {
                        //判断流程是否存在
                        List<Models.DingModels.Tasks> tasksListNew = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.FlowId.ToString() == "6").ToList();
                        if (tasksListNew.Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流水号不存在！", "") { },
                            };
                        }
                        //判断流程是否已结束
                        List<Models.DingModels.Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.IsSend != true && t.State == 0).ToList();
                        if (tasksList.Count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流程未结束！", "") { },
                            };
                        }
                        Models.DingModels.Tasks tasksFinish = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 5).FirstOrDefault();
                        Models.DingModels.Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 0).FirstOrDefault();
                        if (tasks == null)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流水号不存在", "") { },
                            };
                        }

                        Models.DingModels.ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == tasks.ProjectId.ToString()).FirstOrDefault();
                        Models.DingModels.Purchase purchase = context.Purchase.Where(p => p.TaskId == taskId).FirstOrDefault();
                        processingProgress.ProjectType = projectInfo.ProjectType;
                        processingProgress.ProjectSmallType = projectInfo.ProjectSmallType;
                        processingProgress.ProjectId = projectInfo.ProjectId;
                        processingProgress.ProjectName = projectInfo.ProjectName;
                        processingProgress.TaskId = taskId;
                        processingProgress.Bom = projectInfo.ProjectName + "(流水号" + taskId + ")";
                        processingProgress.Designer = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).Designer;
                        processingProgress.DesignerId = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).DesignerId;
                        processingProgress.BomTime = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.TwoD = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.ThreeD = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.NeedTime = purchase.NeedTime;
                        processingProgress.NeedCount = tasks.Remark;
                    }
                    return new NewErrorModel()
                    {
                        data = processingProgress,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
                else
                {
                    ProcessingProgress processingProgress = new ProcessingProgress();
                    using (DDContextHs context = new DDContextHs())
                    {
                        //判断流程是否存在
                        List<Models.DingModelsHs.Tasks> tasksListNew = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.FlowId.ToString() == "6").ToList();
                        if (tasksListNew.Count == 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流水号不存在！", "") { },
                            };
                        }
                        //判断流程是否已结束
                        List<Models.DingModelsHs.Tasks> tasksList = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.IsSend != true && t.State == 0).ToList();
                        if (tasksList.Count > 0)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流程未结束！", "") { },
                            };
                        }
                        Models.DingModelsHs.Tasks tasksFinish = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 5).FirstOrDefault();
                        Models.DingModelsHs.Tasks tasks = context.Tasks.Where(t => t.TaskId.ToString() == taskId && t.NodeId == 0).FirstOrDefault();
                        if (tasks == null)
                        {
                            return new NewErrorModel()
                            {
                                error = new Error(1, "流水号不存在", "") { },
                            };
                        }

                        Models.DingModelsHs.ProjectInfo projectInfo = context.ProjectInfo.Where(p => p.ProjectId == tasks.ProjectId.ToString()).FirstOrDefault();
                        Models.DingModelsHs.Purchase purchase = context.Purchase.Where(p => p.TaskId == taskId).FirstOrDefault();
                        processingProgress.ProjectType = projectInfo.ProjectType;
                        processingProgress.ProjectSmallType = projectInfo.ProjectSmallType;
                        processingProgress.ProjectId = projectInfo.ProjectId;
                        processingProgress.ProjectName = projectInfo.ProjectName;
                        processingProgress.TaskId = taskId;
                        processingProgress.Bom = projectInfo.ProjectName + "(流水号" + taskId + ")";
                        processingProgress.Designer = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).Designer;
                        processingProgress.DesignerId = JsonConvert.DeserializeObject<DesignerModel>(tasks.counts).DesignerId;
                        processingProgress.BomTime = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.TwoD = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.ThreeD = DateTime.Parse(tasksFinish.ApplyTime).ToString("yyyy-MM-dd");
                        processingProgress.NeedTime = purchase.NeedTime;
                        processingProgress.NeedCount = tasks.Remark;
                    }
                    return new NewErrorModel()
                    {
                        data = processingProgress,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取用户权限(返回 0 生产加工进度发起人 1 生产加工进度分配人 2 没权限(设计人员) 3.实际记录人)
        /// </summary>
        /// <param name="applyManId">用户Id</param>
        /// <param name="taskId">可不传(传了只返回当前流水号对应的权限)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPower")]
        public NewErrorModel GetPower(string applyManId, string taskId = "")
        {
            try
            {
                using (DDContext context = new DDContext())
                {
                    List<int> vs = new List<int>();
                    if (!string.IsNullOrEmpty(taskId))
                    {
                        ProcessingProgress processingProgress = context.ProcessingProgress.Where(p => (p.DesignerId.Contains(applyManId) || p.HeadOfDepartmentsId.Contains(applyManId) ||
                       p.NoteTakerId.Contains(applyManId) || p.TabulatorId.Contains(applyManId)) && p.TaskId == taskId).FirstOrDefault();

                        if (applyManId == processingProgress.DesignerId)
                        {
                            vs.Add(2);
                        }
                        if (applyManId == processingProgress.NoteTakerId) //记录人
                        {
                            vs.Add(3);
                        }
                        if (applyManId == processingProgress.TabulatorId) //制表人
                        {
                            vs.Add(0);
                        }
                        if (applyManId == processingProgress.HeadOfDepartmentsId) //分配人
                        {
                            vs.Add(1);
                        }
                    }
                    else
                    {
                        List<PowerModel> powerModels = new List<PowerModel>();
                        List<ProcessingProgress> processingProgressList = context.ProcessingProgress.Where(p => p.DesignerId.Contains(applyManId) || p.HeadOfDepartmentsId.Contains(applyManId) ||
                        p.NoteTakerId.Contains(applyManId) || p.TabulatorId.Contains(applyManId)).ToList();
                        foreach (var processingProgress in processingProgressList)
                        {
                            List<int> vst = new List<int>();
                            PowerModel powerModel = new PowerModel();
                            if (applyManId == processingProgress.DesignerId)
                            {
                                vst.Add(2);
                            }
                            if (applyManId == processingProgress.NoteTakerId) //记录人
                            {
                                vst.Add(3);
                            }
                            if (applyManId == processingProgress.TabulatorId) //制表人
                            {
                                vst.Add(0);
                            }
                            if (applyManId == processingProgress.HeadOfDepartmentsId) //分配人
                            {
                                vst.Add(1);
                            }
                            powerModel.taskId = processingProgress.TaskId;
                            powerModel.vs = vst;
                            powerModels.Add(powerModel);
                        }

                        return new NewErrorModel()
                        {
                            data = powerModels,
                            error = new Error(0, "读取成功！", "") { },
                        };
                    }

                    return new NewErrorModel()
                    {
                        data = vs,
                        error = new Error(0, "读取成功！", "") { },
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class PowerModel
    {
        public string taskId { get; set; }
        public List<int> vs { get; set; }
    }


    public class ProcessingProgressModel
    {
        /// <summary>
        /// 用户Id(当前操作处理人Id)
        /// </summary>
        public string applyManId { get; set; }

        /// <summary>
        /// 用户名(当前操作处理人)
        /// </summary>
        public string applyMan { get; set; }

        /// <summary>
        /// 公司Id  研究院 0 华数 1
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 是否是Excel上传
        /// </summary>
        public bool IsExcelUpload { get; set; }

        public List<ProcessingProgress> processingProgresses { get; set; }

    }

    public class DesignerModel
    {
        public string Designer { get; set; }
        public string DesignerId { get; set; }
    }
}
