using BackEnd;
using System;
using System.Reflection;
using UnityEngine;

namespace ThePocket
{
    public class GameChartQuery
    {
        private IGameChartForAutoGeneration _gameChart;

        public GameChartQuery(IGameChartForAutoGeneration gameChart)
        {
            _gameChart = gameChart;
        }

        public void Load(AfterBackendLoadDelegate afterBackendLoadFunc)
        {
            TheBackendQueryResult result = new();
            result.ClassName = GetType().Name;
            result.FunctionName = MethodBase.GetCurrentMethod()?.Name;
            result.TableName = _gameChart.GetName();

            string chartName = _gameChart.GetName();
            string chartId = GameChartList.GetChartId(chartName);

            if (chartId == null)
            {
                result.IsSuccess = false;
                result.ErrorInfo = $"Not Found Chart File Id. [ ChartName : {chartName} ]";

                afterBackendLoadFunc(result);
                return;
            }

            SendQueue.Enqueue(Backend.Chart.GetChartContents, chartId, callback =>
            {
                try
                {
                    Debug.Log($"Backend.Chart.GetChartContents({chartId}) : {callback}");

                    if (callback.IsSuccess() == false)
                    {
                        throw new Exception(callback.ToString());
                    }

                    LitJson.JsonData gameChartJson = callback.FlattenRows();

                    _gameChart.ToLocal(gameChartJson);

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
    }
}