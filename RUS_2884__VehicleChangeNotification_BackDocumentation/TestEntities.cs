using System;
    using System.Collections.Generic;
    using System.Text;
    using HP.ST.Fwk.RunTimeFWK.Utilities;
    using HP.ST.Fwk.RunTimeFWK.BindingFWK;
    
    namespace Script
    {
    
    public class TestEntities
    {
    public ISTRunTimeContext Context = null;
    public Dictionary<string, HP.ST.Fwk.RunTimeFWK.DataHandling.IDataSource> dataSourceNameToDataSource = new Dictionary<string, HP.ST.Fwk.RunTimeFWK.DataHandling.IDataSource>();
    
    protected HP.ST.Fwk.RunTimeFWK.DataHandling.IDataSource GetDataSource(string dataSourceName)
    {
    if(!dataSourceNameToDataSource.ContainsKey(dataSourceName))
    	throw new Exception(("A data source with the specified name does not exist."));
    return dataSourceNameToDataSource[dataSourceName];
    }
    public HP.ST.Ext.BasicActivities.DataFetchActivity DataFetchActivity56 = null;
    public HP.ST.Ext.BasicActivities.DataFetchActivity DataFetchActivity53 = null;
    public HP.ST.Ext.BasicActivities.StartActivity StartActivity1 = null;
    public HP.ST.Fwk.RunTimeFWK.CompositeActivities.Loop<Loop2Input> Loop2 = null;
    public HP.ST.Ext.BasicActivities.DataExporterActivity DataExporterActivity55 = null;
    public HP.ST.Ext.BasicActivities.DataExporterActivity DataExporterActivity58 = null;
    public HP.ST.Ext.BasicActivities.DataExporterCloseActivity DataExporterCloseActivity59 = null;
    public HP.ST.Ext.BasicActivities.EndActivity EndActivity3 = null;
    public HP.ST.Ext.BasicActivities.DataDisconnectActivity DataDisconnectActivity54 = null;
    public HP.ST.Ext.BasicActivities.DataDisconnectActivity DataDisconnectActivity57 = null;
    public HP.ST.Fwk.RunTimeFWK.CompositeActivities.Sequence Sequence52 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity29Input,CodeActivity29Output> CodeActivity29 = null;
    public HP.ST.Ext.BasicActivities.SetTestVariableActivity SetTestVariableActivity48 = null;
    public HP.ST.Ext.TransformXmlActivity.XMLToStringActivity XMLToStringActivity19 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity17Input,CodeActivity17Output> CodeActivity17 = null;
    public HP.ST.Fwk.RunTimeFWK.CompositeActivities.IfElse<IfElse44Input> IfElse44 = null;
    public HP.ST.Fwk.RunTimeFWK.CompositeActivities.IfElseBranch IfElseBranch45 = null;
    public HP.ST.Fwk.RunTimeFWK.CompositeActivities.IfElseBranch IfElseBranch46 = null;
    public HP.ST.Ext.STRunnerActivity.RunSTActivity CallSTTest47 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity26Input,CodeActivity26Output> CodeActivity26 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity27Input,CodeActivity27Output> CodeActivity27 = null;
    public HP.ST.Ext.STRunnerActivity.RunSTActivity CallSTTest34 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity28Input,CodeActivity28Output> CodeActivity28 = null;
    public HP.ST.Ext.BasicActivities.CodeActivity<CodeActivity51Input,CodeActivity51Output> CodeActivity51 = null;
    
    }
    
    }
    