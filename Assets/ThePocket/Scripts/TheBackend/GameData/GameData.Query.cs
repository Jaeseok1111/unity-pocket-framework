using BackEnd;
using System;
using System.Reflection;
using UnityEngine;

namespace ThePocket
{
    public class GameDataQuery
    {
        private IGameDataForAutoGeneration _gameData;
        private string _inDate;

        public GameDataQuery(IGameDataForAutoGeneration gameData)
        {
            _gameData = gameData;
        }

        public void LoadByMyData(AfterBackendLoadDelegate afterLoad)
        {
            TheBackendQueryResult result = new();
            result.IsSuccess = true;
            result.ClassName = GetType().Name;
            result.FunctionName = MethodBase.GetCurrentMethod()?.Name;
            result.TableName = _gameData.GetName();

            SendQueue.Enqueue(Backend.GameData.GetMyData, result.TableName, new Where(), callback =>
            {
                try
                {
                    Debug.Log($"Backend.GameData.GetMyData({result.TableName}) : {callback}");

                    if (callback.IsSuccess())
                    {
                        if (callback.FlattenRows().Count > 0)
                        {
                            _inDate = callback.FlattenRows()[0]["inDate"].ToString();

                            _gameData.ToLocal(callback.FlattenRows()[0]);
                        }
                    }
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.ErrorInfo = e.ToString();
                }
                finally
                {
                    afterLoad(result);
                }
            });
        }

        public void LoadByTransaction(LitJson.JsonData gameDataJson, AfterBackendLoadDelegate afterLoad)
        {
            TheBackendQueryResult result = new();
            result.IsSuccess = true;
            result.ClassName = GetType().Name;
            result.FunctionName = MethodBase.GetCurrentMethod()?.Name;
            result.TableName = _gameData.GetName();

            try
            {
                _inDate = gameDataJson["inDate"].ToString();

                _gameData.ToLocal(gameDataJson);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ErrorInfo = e.ToString();
            }
            finally
            {
                afterLoad(result);
            }
        }

        public void Update(AfterBackendCallback afterCallback)
        {
            TheBackendQueryResult result = new();
            result.ClassName = GetType().Name;
            result.FunctionName = MethodBase.GetCurrentMethod()?.Name;
            result.TableName = _gameData.GetName();

            SendQueue.Enqueue(Backend.GameData.UpdateV2, result.TableName, _inDate, Backend.UserInDate, _gameData.ToServer(), callback =>
            {
                Debug.Log($"Backend.GameData.UpdateV2({result.TableName}, {_inDate}, {Backend.UserInDate}) : {callback}");

                result.IsSuccess = callback.IsSuccess();

                afterCallback(result);
            });
        }

        public TransactionValue UpdateByTransaction()
        {
            return TransactionValue.SetUpdateV2(_gameData.GetName(), _inDate, Backend.UserInDate, _gameData.ToServer());
        }

        public TransactionValue GetTransactionValue()
        {
            Where where = new Where();
            where.Equal("owner_inDate", Backend.UserInDate);

            return TransactionValue.SetGet(_gameData.GetName(), where);
        }
    }
}