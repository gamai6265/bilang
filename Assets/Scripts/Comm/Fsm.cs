using System;
using System.Collections.Generic;
/**     设计思路，参考：Unity 4.3.2f 版本的Mecanim动画系统中的状态机，
///     根据使用上的功能，来猜想实现思路，当然可能我的实现方式不是最好，
///     如果大家还有比较好的一些见解，那大家一起交流交流吧。
///     该博文就不多说其它的，我的文采也不好，直接上代码吧。
///     1.1：修正：1.0版本的一个Pipeline的多个条件之间为：“与”关系
///     1.3：完善：Transition, ChangedStatePipeline，PipelineCondition与FSM之关系
///     让每个以上三类对象，都分别在各各FSM，或是他们类之间最大化可共用；
///     1.4：优化：FSM中的AnyState与CurState的进入，与离开的触发位置（放在Update中处理，即下一帧中处理）
///     添加：FSM中的Blackboard黑板数据设计（有点像BT树中的黑板数据）
///     )
///     @author :   Jave.Lin(afeng)  modify by danie
///     @time   :   2014-03-11
///     @version:   1.1
///      
**/
namespace Cloth3D{
    /// <summary>
    ///     状态机中，当前状态发生改变的事件委托声明
    /// </summary>
    public delegate void CurStateChangedEventHandler(Fsm sender);

    /// <summary>
    ///     状态机中，当有参数发生变化时的事件委托声明
    /// </summary>
    public delegate void ParamsChangedEventHandler(Fsm sender, ParamsChangedEvent args);

    /// <summary>
    ///     过度参数为函数委托的声明
    /// </summary>
    public delegate bool FuncParamHandler(params object[] objs);

    /// <summary>
    ///     状态机中，当有参数发生变化时的参数类声明
    /// </summary>
    public class ParamsChangedEvent : EventArgs {
        private readonly Param _lastValue;
        private readonly Param _curValue;

        public ParamsChangedEvent(Param lastValue, Param curValue) {
            _lastValue = lastValue;
            _curValue = curValue;
        }

        public Param LastValue {
            get { return _lastValue; }
        }

        public Param CurValue {
            get { return _curValue; }
        }
    }

    /// <summary>
    ///     状态参数检验异常类
    /// </summary>
    public class FsmParamInvalidatedException : Exception {
        public FsmParamInvalidatedException(string msg)
            : base(msg) {
        }
    }

    /// <summary>
    ///     有限状态机
    ///     (
    /// </summary>
    public class Fsm {
        private readonly Dictionary<string, Param> _fsmParams = new Dictionary<string, Param>();

        private readonly Dictionary<string, State> _states = new Dictionary<string, State>();

        public string Name;
        private readonly Blackboard _blackboard = new Blackboard();

        public Fsm()
            : this("Fsm") {
        }

        public Fsm(string name) {
            Name = name;
        }

        public Blackboard Blackboard {
            get { return _blackboard; }
        }

        public State LastState { get; private set; }

        public State CurState { get; set; }

        public State LastAnyState { get; private set; }

        public State AnyState { get; set; }

        /// <summary>
        ///     throw an FsmParamInvalidatedException
        /// </summary>
        public static void CheckParamValidated(Fsm fsm, string name, object args, ConditionType type) {
            if ((type != ConditionType.Func || type == ConditionType.None) && args == null)
                throw new FsmParamInvalidatedException("ChangStatePipeline add condition args can not be null");
            var srcParam = fsm.GetParam(name);
            if (srcParam == null)
                throw new FsmParamInvalidatedException(string.Format("fsm not contains param : {0}", name));
            if (!srcParam.CheckValue(args))
                throw new FsmParamInvalidatedException(
                    string.Format("ChangStatePipeline add condition args ValidatedSelf invalidated : {0}", args));
        }

        public static void AddTransitionTo(Fsm fsm, string srcStateName, string targetStateName, Condition[] conditions) {
            AddTransitionTo(fsm, srcStateName, targetStateName, new List<Condition>(conditions), true);
        }

        public static void AddTransitionTo(Fsm fsm, string srcStateName, string targetStateName, List<Condition> conditions) {
            AddTransitionTo(fsm, srcStateName, targetStateName, conditions, true);
        }

        public static void AddTransitionTo(Fsm fsm, string srcStateName, string targetStateName, List<Condition> conditions,
            bool addInSameStateNameTransition) {
            Transition transition;
            Pipeline pipeline;
            GetTransitionAndPipeline(fsm, srcStateName, targetStateName, addInSameStateNameTransition, out transition,
                out pipeline);
            pipeline.AddConditions(conditions);
        }

        public static void GetTransitionAndPipeline(Fsm fsm, string srcStateName, string targetName,
            bool addInSameStateNameTransition, out Transition out1, out Pipeline out2) {
            out1 = null;
            out2 = null;

            var srcState = fsm.GetState(srcStateName);
            if (srcState == null)
                return;
            if (addInSameStateNameTransition) {
                var ts = srcState.GetTransition(srcStateName, targetName);
                if (ts.Count > 0)
                    out1 = ts[0];
                if (out1 != null) {
                    var ps = out1.GetPipeline(srcStateName, targetName);
                    if (ps.Count > 0)
                        out2 = ps[0];
                }
            }
            if (out1 == null) {
                out1 = new Transition(srcStateName, targetName);
                srcState.AddTransition(out1);
            }
            if (out2 == null) {
                out2 = new Pipeline(srcStateName, targetName);
                out1.AddPipeline(out2);
            }
        }

        public event CurStateChangedEventHandler CurStateChangedEvent;
        public event ParamsChangedEventHandler ParamsChangedEvent;

        public Param AddParam(string name, ParamType type) {
            var result = _fsmParams.ContainsKey(name) ? _fsmParams[name] : null;

            if (result != null) {
                if (result.Type != type)
                    throw new Exception(
                        string.Format(
                            "Fsm AddParam Name : {0}, is already def, but still add param same Name, but type different, src type : {1}, new type : {2}",
                            result.Type, type));
            }

            if (result == null) {
                result = new ParamValue(name) {Type = type};
                switch (type) {
                    case ParamType.Int:
                        result.Value = 0;
                        break;
                    case ParamType.Float:
                        result.Value = 0f;
                        break;
                    case ParamType.Double:
                        result.Value = 0D;
                        break;
                    case ParamType.Boolean:
                        result.Value = false;
                        break;
                    case ParamType.Func:
                        result.Value = null;
                        break;
                }
                AddParam(result);
            }

            return result;
        }

        public void AddParam(Param param) {
            if (param == null || _fsmParams.ContainsKey(param.Name))
                return;
            if (!param.CheckValue())
                throw new Exception(string.Format("fsm add param ValidatedSelf invalidated : {0}", param));
            _fsmParams.Add(param.Name, param);
        }

        public void RemoveParam(Param param) {
            if (param == null || !_fsmParams.ContainsKey(param.Name))
                return;
            _fsmParams.Remove(param.Name);
        }

        public T GetParam<T>(string name) where T : Param {
            return GetParam(name) as T;
        }

        public T GetParamValue<T>(string name) where T : struct {
            return GetParam(name).GetValue<T>();
        }

        public T GetParamClassValue<T>(string name) where T : class {
            return GetParam(name).GetClassValue<T>();
        }

        public Param GetParam(string name) {
            if (!_fsmParams.ContainsKey(name))
                return null;
            return _fsmParams[name];
        }

        public void SetParamValue(string name, object value) {
            SetParamValue(name, value, true);
        }

        public void SetParamValue(string name, object value, bool autoAsValue) {
            var srcParam = GetParam(name);
            if (srcParam == null)
                throw new Exception(string.Format("Fsm.SetParamValue param : Name : {0} is not contains", name));

            if (autoAsValue && value.GetType() != srcParam.Value.GetType()) {
                try {
                    switch (srcParam.Type) {
                        case ParamType.Int:
                            value = Convert.ToInt32(value);
                            break;
                        case ParamType.Float:
                            value = Convert.ToSingle(value);
                            break;
                        case ParamType.Double:
                            value = Convert.ToDouble(value);
                            break;
                        case ParamType.Func:
                            value = (Delegate) value;
                            break;
                        default:
                            throw new Exception(string.Format("Fsm.SetParamValue new param type unhandled, type : {0}",
                                srcParam.Type));
                    }
                } catch (Exception er) {
                    throw new Exception(
                        string.Format(
                            "Fsm.SetParamValue param : Name : {0}, value is invalidated : {1}\nException:{2}", name,
                            value, er));
                }
            }

            if (!srcParam.CheckValue(value))
                throw new Exception(string.Format("Fsm.SetParamValue param : Name : {0}, value is invalidated : {1}",
                    name, value));

            var lastValue = srcParam.Clone();
            srcParam.Value = value;
            OnParamsChangedEvent(new ParamsChangedEvent(lastValue, srcParam));
            if (CurState != null)
                CurState.DirtyState();
        }

        public void AddState(State state) {
            if (_states.ContainsKey(state.Name)) return;
            _states.Add(state.Name, state);
        }

        public void RemoveState(State state) {
            if (state == null || !_states.ContainsKey(state.Name)) return;
            _states.Remove(state.Name);
        }

        public State GetState(string name) {
            if (!_states.ContainsKey(name))
                return null;
            return _states[name];
        }

        public void Update() {
            if (AnyState != LastAnyState) {
                if (LastAnyState != null) {
                    LastAnyState.LeaveState(this);
                }
                LastAnyState = AnyState;
                if (AnyState != null) {
                    AnyState.EnterState(this);
                }
            }
            if (AnyState != null) {
                AnyState.Update(this);
                AnyState.Excute(this);
            }

            if (CurState != LastState) {
                if (LastState != null) {
                    LastState.LeaveState(this);
                }
                LastState = CurState;
                if (CurState != null) {
                    CurState.EnterState(this);
                }
            }
            if (CurState != null && CurState != AnyState) {
                CurState.Update(this);
                CurState.Excute(this);
            }
        }

        public void SetCurState(string stateName, bool force=false) {
            var state = GetState(stateName);
            if (force && state == LastState) {
                LastState = null;
            }
            CurState = GetState(stateName);
        }

        private void OnCurStateChangedEvent() {
            if (CurStateChangedEvent != null) {
                CurStateChangedEvent(this);
            }
        }

        private void OnParamsChangedEvent(ParamsChangedEvent args) {
            if (ParamsChangedEvent != null) {
                ParamsChangedEvent(this, args);
            }
        }

        public override string ToString() {
            return string.Format("fsm : {0}", Name);
        }
    }

    /// <summary>
    ///     状态
    /// </summary>
    public abstract class State {
        protected bool _isInited;

        private bool _needUpdate = true;
        private readonly List<Transition> _transitions = new List<Transition>();
        private readonly string _name;

        public State(string name) {
            _name = name;
        }

        public List<Transition> Transitions {
            get { return _transitions; }
        }

        public string Name {
            get { return _name; }
        }

        public bool IsInited {
            get { return _isInited; }
        }

        public List<Transition> GetTransition(string srcStateName, string targetStateName) {
            var result = new List<Transition>();
            for (int i = 0, len = Transitions.Count; i < len; i++) {
                var t = Transitions[i];
                if (t.SrcStateName == srcStateName && t.TargetStateName == targetStateName)
                    result.Add(t);
            }
            return result;
        }

        public void AddTransition(Transition value) {
            if (Transitions.Contains(value))
                return;
            Transitions.Add(value);
        }

        public void RemoveTransition(Transition value) {
            if (!Transitions.Contains(value))
                return;
            Transitions.Remove(value);
        }

        public override string ToString() {
            return string.Format("state Name : {0}", Name);
        }

        /// <summary>
        ///     如果调用之后，就会对Update执行一次
        /// </summary>
        public void DirtyState() {
            _needUpdate = true;
        }

        public void Update(Fsm fsm) {
            if (!_isInited) {
                _isInited = true;
                Init(fsm);
            }

            if (!_needUpdate) return;
            _needUpdate = false;

            foreach (var t in Transitions) {
                t.Update(fsm);
            }
        }

        /// <summary>
        ///     First time to Update will trigger
        /// </summary>
        protected abstract void Init(Fsm fsm);

        public abstract void EnterState(Fsm fsm);

        public abstract void LeaveState(Fsm fsm);

        public abstract void Excute(Fsm fsm);
    }

    /// <summary>
    ///     状态之间的过度处理(可包含多个过度管道)
    /// </summary>
    public class Transition {
        private readonly string _srcStateName;
        private readonly string _targetStateName;
        private readonly List<Pipeline> _pipelines = new List<Pipeline>();

        public Transition(string srcStateName, string targetStateName) {
            _srcStateName = srcStateName;
            _targetStateName = targetStateName;
        }

        public string SrcStateName {
            get { return _srcStateName; }
        }

        public string TargetStateName {
            get { return _targetStateName; }
        }

        public List<Pipeline> Pipelines {
            get { return _pipelines; }
        }

        public List<Pipeline> GetPipeline(string srcStateName, string targetStateName) {
            var result = new List<Pipeline>();
            for (int i = 0, len = Pipelines.Count; i < len; i++) {
                var t = Pipelines[i];
                if (t.SrcStateName == srcStateName && t.TargetStateName == targetStateName)
                    result.Add(t);
            }
            return result;
        }

        public void AddPipeline(Pipeline pipeline) {
            if (Pipelines.Contains(pipeline)) return;
            Pipelines.Add(pipeline);
        }

        public void AddPipelines(List<Pipeline> pipelines) {
            foreach (var p in pipelines)
                AddPipeline(p);
        }

        public void RemovePipeline(Pipeline pipeline) {
            if (!Pipelines.Contains(pipeline)) return;
            Pipelines.Remove(pipeline);
        }

        public void Update(Fsm fsm) {
            foreach (var p in Pipelines) {
                if (p.Check(fsm)) {
                    fsm.SetCurState(p.TargetStateName);
                    return;
                }
            }
        }

        public void ClearPipelines() {
            Pipelines.Clear();
        }
    }

    /// <summary>
    ///     状态之间的过度管道（可包含多个过度条件）
    ///     1.1版，修正了，对条件之间的控制为：&& ： 与关系
    /// </summary>
    public class Pipeline {
        private readonly List<Condition> _conditions = new List<Condition>();
        private readonly string _srcStateName;

        public Pipeline(string srcStateName, string targetStateName) {
            _srcStateName = srcStateName;
            TargetStateName = targetStateName;
        }

        public string SrcStateName {
            get { return _srcStateName; }
        } //TODO danie pineline依附于 transition,两者的功能重复，删除其一

        public string TargetStateName { get; set; }

        /// <summary>
        ///     调用该方法前，最好先调用CheckAndThrowException
        /// </summary>
        public Condition AddCondition(string name, object args, ConditionType type) {
            var result = new Condition(name, args, type);
            _conditions.Add(result);
            return result;
        }

        public void AddConditions(List<Condition> conditions) {
            foreach (var c in conditions)
                AddCondition(c);
        }

        /// <summary>
        ///     调用该方法前，最好先调用CheckAndThrowException
        /// </summary>
        public void AddCondition(Condition condition) {
            if (_conditions.Contains(condition))
                return;
            _conditions.Add(condition);
        }

        public void RemoveCondition(int idx) {
            _conditions.RemoveAt(idx);
        }

        public void RemoveCondition(Condition condition) {
            if (!_conditions.Contains(condition))
                return;
            _conditions.Remove(condition);
        }

        public bool Check(Fsm fsm) {
            // 1.0的时候是这样的：“或”关系
            //foreach (var condition in _conditions)
            //{
            //    if (condition.Check())
            //        return true;
            //}

            // 1.1改为：以下：“与”关系
            var result = true;
            foreach (var condition in _conditions) {
                if (!condition.Check(fsm)) {
                    // 只要有一个不成立，则不通过
                    result = false;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    ///     状态之间过度管道中的单个过度条件
    /// </summary>
    public class Condition {
        private readonly string _paramName;
        private readonly ConditionType _conditionType;
        private readonly object _args;

        public Condition(string paramName, object args, ConditionType conditionType) {
            _paramName = paramName;
            _args = args;
            _conditionType = conditionType;
        }

        public string ParamName {
            get { return _paramName; }
        }

        public ConditionType ConditionType {
            get { return _conditionType; }
        }

        public object Args {
            get { return _args; }
        }

        public bool Check(Fsm fsm) {
            if (string.IsNullOrEmpty(ParamName))
                return false;

            var param = fsm.GetParam<Param>(ParamName);
            if (param == null)
                return false;

            var result = false;
            switch (param.Type) {
                case ParamType.Int:
                case ParamType.Float:
                case ParamType.Double:
                    var valueParam = fsm.GetParam<ParamValue>(ParamName);
                    if (valueParam != null) {
                        result = CheckDouble(Convert.ToDouble(valueParam.Value), Convert.ToDouble(Args), ConditionType);
                    }
                    break;
                case ParamType.Boolean:
                    var boolParam = fsm.GetParam<ParamBoolean>(ParamName);
                    if (boolParam != null) {
                        result = CheckBool(Convert.ToBoolean(boolParam.Value), Convert.ToBoolean(Args), ConditionType);
                    }
                    break;
                case ParamType.Func:
                    var funcParam = fsm.GetParam<ParamFunc>(ParamName);
                    if (funcParam != null) {
                        var action = (FuncParamHandler) funcParam.Value;
                        var funcArgs = Args == null ? null : (Args is object[] ? Args as object[] : new[] {Args});
                        result = action != null ? action(funcArgs) : false;
                    }
                    break;
                default:
                    throw new Exception(
                        string.Format("PipelineCondition.Check(Fsm fsm) paramName : {0}, paramType : {1} unhandler!",
                            ParamName, param.Type));
            }

            return result;
        }

        private static bool CheckBool(bool a, bool b, ConditionType type) {
            if (type == ConditionType.Equals)
                return a == b;
            if (type == ConditionType.NotEquals)
                return a != b;
            return false;
        }

        public static bool CheckInt(int a, int b, ConditionType type) {
            return CheckValueAndTarget(a, b, type);
        }

        public static bool CheckFloat(float a, float b, ConditionType type) {
            return CheckValueAndTarget(a, b, type);
        }

        private static bool CheckDouble(double a, double b, ConditionType type) {
            return CheckValueAndTarget(a, b, type);
        }

        private static bool CheckValueAndTarget(double value, double target, ConditionType type) {
            var result = false;
            if (ContainType(type, ConditionType.Less)) {
                result = value < target;
            } else if (ContainType(type, ConditionType.LessEquals)) {
                result = value <= target;
            } else if (ContainType(type, ConditionType.Equals)) {
                result = value == target;
            } else if (ContainType(type, ConditionType.Greater)) {
                result = value > target;
            } else if (ContainType(type, ConditionType.GreaterEquals)) {
                result = value >= target;
            } else if (ContainType(type, ConditionType.NotEquals)) {
                result = value != target;
            }
            return result;
        }

        public static bool ContainType(ConditionType type, ConditionType beContainedType) {
            return (type & beContainedType) == beContainedType;
        }
    }

    /// <summary>
    ///     过度参数的成立条件类型
    /// </summary>
    [Flags]
    public enum ConditionType {
        None = 0,
        Less = 1,
        LessEquals = 2,
        Equals = 4,
        Greater = 8,
        GreaterEquals = 16,
        NotEquals = 32,
        Func = 64
    }

    /// <summary>
    ///     过度参数类型
    /// </summary>
    public enum ParamType {
        Int,
        Float,
        Double,
        Boolean,
        Func
    }

    /// <summary>
    ///     过度参数是函数类型
    /// </summary>
    public class ParamFunc : Param {
        public ParamFunc(string name, Delegate func)
            : base(name, ParamType.Func) {
            Value = func;
        }
    }

    /// <summary>
    ///     过度参数是布尔类型
    /// </summary>
    public class ParamBoolean : Param {
        public ParamBoolean(string name)
            : base(name, ParamType.Boolean) {
            Value = false;
        }
    }

    /// <summary>
    ///     过度参数是值类型
    /// </summary>
    public class ParamValue : Param {
        public ParamValue(string name)
            : this(name, ParamType.Int) {
        }

        public ParamValue(string name, ParamType type)
            : base(name, type) {
            if (type == ParamType.Func)
                throw new Exception(string.Format("FSMParamValue type invalidated, type : {0}", type));
            switch (type) {
                case ParamType.Int:
                    Value = 0;
                    break;
                case ParamType.Float:
                    Value = 0f;
                    break;
                case ParamType.Double:
                    Value = 0d;
                    break;
                default:
                    throw new Exception(string.Format("FSMParamValue new type : {0} unhandled", type));
            }
        }
    }

    /// <summary>
    ///     过度的参数类
    /// </summary>
    public abstract class Param {
        private static readonly Dictionary<ParamType, ConditionType> Dic = new Dictionary<ParamType, ConditionType>();

        public ParamType Type;
        public object Value; // object Value 可以考虑使用FSMParam<T> => Generic Type
        private readonly string _name;

        static Param() {
            Dic[ParamType.Int] =
                Dic[ParamType.Float] =
                    Dic[ParamType.Double] = ConditionType.Less | ConditionType.LessEquals | ConditionType.Equals |
                                            ConditionType.Greater | ConditionType.GreaterEquals |
                                            ConditionType.NotEquals;
            Dic[ParamType.Func] = ConditionType.Func;
            Dic[ParamType.Boolean] = ConditionType.Equals | ConditionType.NotEquals;
        }

        public Param(string name, ParamType type) {
            _name = name;
            Type = type;
        }

        public string Name {
            get { return _name; }
        }

        public static ConditionType GetConditionTypeByParamType(ParamType type) {
            return Dic[type];
        }

        public T GetValue<T>() where T : struct {
            return (T) Value;
        }

        public T GetClassValue<T>() where T : class {
            return Value as T;
        }

        // self
        public bool CheckValue() {
            return CheckValue(Value);
        }

        // sepecial
        public bool CheckValue(object value) {
            var result = false;
            switch (Type) {
                case ParamType.Int:
                    result = value is int;
                    break;
                case ParamType.Float:
                    result = value is float;
                    break;
                case ParamType.Double:
                    result = value is double;
                    break;
                case ParamType.Boolean:
                    result = value is bool;
                    break;
                case ParamType.Func:
                    result = true; // func args, it can spectial anything..., always return true
                    break;
            }
            return result;
        }

        public Param Clone() {
            return MemberwiseClone() as Param;
        }

        public override string ToString() {
            return string.Format("Name : {0}, type : {1}, value : {2}", Name, Type, Value);
        }
    }

    /// <summary>
    ///     黑板数据的更新类型
    /// </summary>
    public enum BlackboardUpdateType {
        Add,
        Remove,
        Modify
    }

    /// <summary>
    ///     FSM中的黑板数据，便于，绑定外部数据，方便在FSM中对外部数据的获取与判断而使用
    ///     有点像BT（Behavior Tree）中的 Black board而设计
    /// </summary>
    public class Blackboard {
        private readonly Dictionary<string, object> _datasStore = new Dictionary<string, object>();

        public bool HaveDatas {
            get { return _datasStore.Keys.Count > 0; }
        }

        public event Action<Blackboard, BlackboardUpdateType> UpdateEvent;

        private void OnUpdateEvent(BlackboardUpdateType updateType) {
            if (UpdateEvent != null) {
                UpdateEvent(this, updateType);
            }
        }

        public T GetData<T>(string name) {
            return (T) _datasStore[name];
        }

        public void AddData(string name, object data) {
            var srcHad = _datasStore.ContainsKey(name);
            _datasStore[name] = data;
            OnUpdateEvent(srcHad ? BlackboardUpdateType.Modify : BlackboardUpdateType.Add);
        }

        public object RemoveData(string name) {
            if (!_datasStore.ContainsKey(name))
                return null;
            var result = _datasStore[name];
            _datasStore.Remove(name);
            OnUpdateEvent(BlackboardUpdateType.Remove);
            return result;
        }

        public void UpdateData(string name) {
            if (!_datasStore.ContainsKey(name))
                return;
            OnUpdateEvent(BlackboardUpdateType.Modify);
        }
    }
}