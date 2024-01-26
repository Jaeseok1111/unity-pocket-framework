using BackEnd;
using System;
using System.Reflection;
using UnityEngine;

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
        BackendQueryResult result = new();
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
                    // 불러온 데이터가 하나라도 존재할 경우
                    if (callback.FlattenRows().Count > 0)
                    {
                        // 이후 업데이트에 사용될 각 데이터의 indate값 저장
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
        BackendQueryResult result = new();
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
        BackendQueryResult result = new();
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
