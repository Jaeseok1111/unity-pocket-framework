using BackEnd;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ThePocket
{
    public class GameChartList
    {
        private static readonly Dictionary<string, string> _charts = new();

        public static void Load(AfterBackendLoadDelegate afterBackendLoadFunc)
        {
            TheBackendQueryResult result = new();
            result.ClassName = "GameChartList";
            result.FunctionName = MethodBase.GetCurrentMethod()?.Name;

            SendQueue.Enqueue(Backend.Chart.GetChartList, callback =>
            {
                try
                {
                    Debug.Log($"Backend.Chart.GetChartList : {callback}");

                    if (callback.IsSuccess() == false)
                    {
                        throw new Exception(callback.ToString());
                    }

                    LitJson.JsonData json = callback.FlattenRows();

                    for (int index = 0; index < json.Count; index++)
                    {
                        string chartName = json[index]["chartName"].ToString();
                        string selectedChartFileId = json[index]["selectedChartFileId"].ToString();

                        if (_charts.ContainsKey(chartName))
                        {
                            Debug.LogWarning($"The same chart key values exist. : {chartName} - {selectedChartFileId}");
                        }
                        else
                        {
                            _charts.Add(chartName, selectedChartFileId);
                        }
                    }

                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrorInfo = ex.Message;
                }
                finally
                {
                    afterBackendLoadFunc(result);
                }
            });
        }

        public static string GetChartId(string chartName)
        {
            return _charts.GetValueOrDefault(chartName, null);
        }
    }
}