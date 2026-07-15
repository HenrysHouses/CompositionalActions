#nullable disable
#if UNITY_EDITOR

namespace HH.CompositionalActions.Editor
{
    using System;
    using System.Collections.Generic;
    using Bewildered.Editor;
    using HH.CompositionalActions;
    using HH.CompositionalActions.Contracts;
    using HH.Editor;
    using HH.EditorExtensions;
    using UnityEditor;
    using UnityEngine;
    using static HH.Editor.HEditor;

    [CustomPropertyDrawer(typeof(ActionInvoker))]
    public class ActionInvokerPropertyDrawer : PropertyDrawer
    {
        private readonly Color completedColor = new Color(0, 0.4f, 0);
        private readonly Color executingColor = new Color(0.6f, 0.6f, 0);
        private readonly Color notReachedColor = new Color(0.2f, 0.2f, 0.2f);
        private readonly Color failedColor = new Color(0.6f, 0, 0);
        private readonly Dictionary<ActionInvoker, List<ActionInvoker.ExecutionState>> executionStates = new Dictionary<ActionInvoker, List<ActionInvoker.ExecutionState>>();
        private readonly Dictionary<ActionInvoker.ExecutionState, TableGUI> drawnTables = new Dictionary<ActionInvoker.ExecutionState, TableGUI>();
        private readonly Dictionary<Type, MonoScript> monoScripts = new Dictionary<Type, MonoScript>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int originalIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            position = DrawBackground(position, 3);

            position = SetStandardHeight(position, 1);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, new GUIContent($"{property.displayName} Debugger"));
            position = PushLine(position);

            ActionInvoker target = (ActionInvoker)property.GetValue();

            if (target.EditorExecutionStateHook != StoreInvokedExecutionState)
            {
                target.EditorExecutionStateHook = StoreInvokedExecutionState;
            }

            if (property.isExpanded)
            {
                DrawDebugger(position, target);
            }

            EditorGUI.indentLevel = originalIndent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float backgroundHeight = EditorGUIUtility.standardVerticalSpacing * 4;
            float extraBottomSpacing = EditorGUIUtility.standardVerticalSpacing * 5;

            if (!property.isExpanded)
            {
                return CalculateHeightWithSpacing(1) + extraBottomSpacing + backgroundHeight;
            }

            ActionInvoker script = (ActionInvoker)property.GetValue();
            int historyLines = script.ShowHistory ? script.DebugActionHistory.Length : 0;

            if (!executionStates.ContainsKey(script))
            {
                return CalculateHeightWithSpacing(5 + historyLines) + extraBottomSpacing + backgroundHeight;
            }

            int scriptObjectReferenceLine = 2;
            int dropdownHeaders = 3;
            float executionTableHeightOffset = 0;

            TableRect[] tables = new TableRect[executionStates[script].Count];

            Rect executionTable = default;

            if (script.ShowExecution && executionStates.ContainsKey(script))
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    ActionInvoker.ExecutionState state = executionStates[script][i];

                    if (state.Actions == null)
                    {
                        continue;
                    }

                    // BUG null referenec
                    int tableRows = state.Actions.Count + 1;
                    executionTable.height = CalculateHeightWithSpacing(tableRows);

                    bool tableExists = drawnTables.ContainsKey(state);
                    bool[] expandedCells = tableExists ? drawnTables[state].IsExpanded : new bool[state.ActionResults.Count + 1];
                    object[,] privateFieldCells = tableExists ? drawnTables[state].DrawCallCache : new object[tableRows, 2];
                    tables[i] = GenerateTable(executionTable, tableRows, 3, state.ActionResults.ToArray(), expandedCells, privateFieldCells);
                    executionTable = tables[i].OutlineRect;
                    executionTableHeightOffset += executionTable.height + CalculateHeightWithSpacing(1) + 4;
                }
            }

            executionTableHeightOffset += EditorGUIUtility.standardVerticalSpacing * 2;
            executionTableHeightOffset += script.ShowExecution ? -EditorGUIUtility.singleLineHeight : EditorGUIUtility.standardVerticalSpacing * 2;

            float lineHeights = CalculateHeightWithSpacing(
                    /* Height with spacing */
                    scriptObjectReferenceLine
                    + dropdownHeaders
                    + historyLines)
                /* Other heights */
                + extraBottomSpacing
                + backgroundHeight
                + executionTableHeightOffset;

            return lineHeights;
        }

        private void DrawDebugger(Rect position, ActionInvoker script)
        {
            GUI.enabled = false;

            MonoScript interfaceScript, invokerImplementation;
            Type debuggerTarget = script.GetDebuggerTarget();
            Type debuggerType = script.GetType();
            if (!monoScripts.ContainsKey(debuggerTarget))
            {
                interfaceScript = MonoScriptUtility.FindMonoScriptFromType(debuggerTarget);

                if (interfaceScript != null)
                {
                    monoScripts.Add(debuggerTarget, interfaceScript);
                }
            }
            else
            {
                interfaceScript = monoScripts[debuggerTarget];
            }

            if (!monoScripts.ContainsKey(debuggerType))
            {
                invokerImplementation = MonoScriptUtility.FindMonoScriptFromType(debuggerType);

                if (invokerImplementation != null)
                {
                    monoScripts.Add(debuggerType, invokerImplementation);
                }
            }
            else
            {
                invokerImplementation = monoScripts[debuggerType];
            }

            // NOTE: Drawing these monoscripts sould be extracted to a method
            if (invokerImplementation == null)
            {
                DrawErrorBox(position, $"Could not find the MonoScript that contains the invoker implementation \"{debuggerType.Name}\"");
                position = PushLine(position);
            }
            else
            {
                EditorGUI.ObjectField(
                        position,
                        new GUIContent("Invoker Type", "The MonoScript that implements the action invoker"),
                        invokerImplementation,
                        typeof(MonoScript),
                        false);
                position = PushLine(position);
                position.y += EditorGUIUtility.standardVerticalSpacing * 2;
            }

            if (interfaceScript == null)
            {
                DrawErrorBox(position, $"Could not find the MonoScript that contains the target interface \"{debuggerTarget.Name}\"");
                position = PushLine(position);
            }
            else
            {
                EditorGUI.ObjectField(
                        position,
                        new GUIContent("Debugger Target", "The MonoScript that implements the debugged action interface type"),
                        interfaceScript,
                        typeof(MonoScript),
                        false);
                position = PushLine(position);
                position.y += EditorGUIUtility.standardVerticalSpacing * 2;
            }

            if (executionStates.Keys.Count > 0)
            {
                position = DrawExecutionDropdown(position, script);
            }
            else
            {
                EditorGUI.LabelField(position, "No Active Debugger");
                position = PushLine(position);
            }

            position.y += EditorGUIUtility.standardVerticalSpacing;
            DrawHistoryDropdown(position, script);

            GUI.enabled = true;
        }

        private Rect DrawExecutionDropdown(Rect position, ActionInvoker script)
        {
            position.y += EditorGUIUtility.standardVerticalSpacing * 2;
            Rect propertyRect = position;
            Rect propertyContent = Shrink(propertyRect, 2);

            // Execution background Height
            // +1 for the header
            float totalTablesHeight = 1;

            TableRect[] tables = new TableRect[0];
            // BUG: sometimes this says script wasnt in the dictionary
            if (executionStates.ContainsKey(script))
            {
                tables = new TableRect[executionStates[script].Count];
            }

            Rect executionTable = propertyContent;
            executionTable.y += CalculateHeightWithSpacing(1);

            if (script.ShowExecution && executionStates.ContainsKey(script))
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    if (i > 0)
                    {
                        // + spacing between tables
                        executionTable.y += executionTable.height + CalculateHeightWithSpacing(1);
                    }

                    ActionInvoker.ExecutionState state = executionStates[script][i];
                    if (state.Actions == null)
                    {
                        continue;
                    }

                    int tableRows = state.Actions.Count + 1;
                    executionTable.height = CalculateHeightWithSpacing(tableRows);

                    bool tableExists = drawnTables.ContainsKey(state);
                    bool[] expandedCells = tableExists ? drawnTables[state].IsExpanded : new bool[state.ActionResults.Count + 1];
                    object[,] privateFieldCells = tableExists ? drawnTables[state].DrawCallCache : new object[tableRows, 2];

                    tables[i] = GenerateTable(executionTable, tableRows, 3, state.ActionResults.ToArray(), expandedCells, privateFieldCells);
                    executionTable = tables[i].OutlineRect;
                    totalTablesHeight += executionTable.height + CalculateHeightWithSpacing(1) + 4;
                }
            }

            // Draw Execution Dropdown
            propertyContent.height = script.ShowExecution ?
                totalTablesHeight : CalculateHeightWithSpacing(1);

            propertyRect.height = propertyContent.height;

            if (tables.Length > 0)
            {
                DrawBackground(propertyRect);

                GUI.enabled = true;
                Rect executionFoldout = propertyContent;
                executionFoldout.height = CalculateHeightWithSpacing(1);
                script.ShowExecution = EditorGUI.Foldout(executionFoldout, script.ShowExecution, new GUIContent("Execution"));
                GUI.enabled = false;
            }

            if (script.ShowExecution && executionStates.ContainsKey(script))
            {
                for (int i = 0; i < executionStates[script].Count; i++)
                {
                    DrawExecutionTable(tables[i], executionStates[script][i]);
                }
            }

            propertyRect.y += propertyRect.height;
            return propertyRect;
        }

        private TableRect GenerateTable(Rect position, int rows, float outline, IActionResult[] results, bool[] isExpanded, object[,] privateFields)
        {
            Dictionary<int, float> variableHeight = new Dictionary<int, float>();

            for (int i = 0; i < results.Length; i++)
            {
                privateFields[i + 1, 1] ??= false;
                float height = HGUI.GetClassFieldHeight(results[i].GetType(), isExpanded[i + 1], (bool)privateFields[i + 1, 1]);
                variableHeight.Add(i + 1, height);
            }

            TableRect rect = new TableRect(rows, 2, position, outline, variableHeight);
            return rect;
        }

        private Rect DrawExecutionTable(TableRect table, ActionInvoker.ExecutionState state)
        {
            if (table.Rows == null)
            {
                return table.Position;
            }

            GUI.enabled = true;

            // Table
            Color tableColor = (state.IsFinished, state.IsFailed) switch
            {
                (false, false) => executingColor,
                (true, false) => completedColor,
                (true, true) => failedColor,
                (false, true) => failedColor,
            };

            BackgroundGUI tableBackground = new BackgroundGUI(BaseBackgroundColor, tableColor);
            TableGUI gui = GenerateTableGUI(table, tableBackground, state);

            if (!drawnTables.ContainsKey(state))
            {
                drawnTables.Add(state, gui);
            }
            else
            {
                TableGUI updated = gui;
                gui = drawnTables[state];
                gui.Update(updated);
            }

            DrawTable(gui, 1);
            GUI.enabled = false;
            // Return the bottom height position of the final table element. this is actually wrong. it should be position of the bottom of the status rect.
            Rect tableProperty = table.Position;
            tableProperty.y += tableProperty.height;
            return tableProperty;
        }

        private TableGUI GenerateTableGUI(TableRect table, BackgroundGUI tableBackground, ActionInvoker.ExecutionState state)
        {
            TableGUI gui = new TableGUI(table, tableBackground);

            // Header Icons
            GUIContent actionStateIcon = EditorGUIUtility.IconContent("d_AnimatorStateTransition Icon");
            GUIContent actionResultIcon = EditorGUIUtility.IconContent("InProject-Selected-Focused@2x");
            // Header Content
            GUIContent actionTitle = new GUIContent("Action State", actionStateIcon.image);
            GUIContent resultTitle = new GUIContent("Execution Result", actionResultIcon.image);
            gui.CellGUI[0, 0].GUIDrawCall = (rect, expanded, cache) =>
            {
                EditorGUI.LabelField(rect, actionTitle);
                return (expanded, null);
            };
            gui.CellGUI[0, 1].GUIDrawCall = (rect, expanded, cache) =>
            {
                EditorGUI.LabelField(rect, resultTitle);
                return (expanded, null);
            };

            for (int i = 0; i < state.Actions.Count; i++)
            {
                GetActionCellGUI(state, i, out Color actionColor, out GUIContent actionCellContent);

                // NOTE in a delegate like this, if the getter of the variable is inside of the delegate,
                // it will also be stored in the delegate. We only want the final value to be passed to it to avoid errors
                Func<Rect, bool, object, (bool IsExpanded, object ReturnCache)> stateDrawCall = (rect, isExpanded, cache) =>
                {
                    EditorGUI.LabelField(rect, actionCellContent);
                    return (isExpanded, null);
                };

                Func<Rect, bool, object, (bool IsExpanded, object ReturnCache)> resultDrawCall = (rect, isExpanded, cache) =>
                {
                    EditorGUI.LabelField(rect, "No Results Found");
                    return (isExpanded, null);
                };

                if (i < state.ActionResults.Count)
                {
                    IActionResult resultData = state.ActionResults[i];
                    GUIContent label = new GUIContent("Result: " + resultData.GetType().Name);

                    if (resultData != null)
                    {
                        resultDrawCall = (rect, expanded, showPrivateCache) =>
                        {
                            showPrivateCache ??= false;

                            return HGUI.ClassField(rect, label, resultData, expanded, (bool)showPrivateCache);
                        };
                    }
                }

                // SerializedObject serializedResult = CreateSerializedObject(resultData);
                // TODO make a serializedinterface on the action invoker so it can be used to get a serialized property and draw it

                gui.CellGUI[i + 1, 0] = new CellGUI(stateDrawCall, BaseBackgroundColor, actionColor);
                gui.CellGUI[i + 1, 1] = new CellGUI(resultDrawCall, BaseBackgroundColor, actionColor);
            }

            return gui;
        }

        private void GetActionCellGUI(ActionInvoker.ExecutionState state, int target, out Color actionState, out GUIContent label)
        {
            actionState = notReachedColor;
            GUIContent inactiveIcon = EditorGUIUtility.IconContent("DotFrameDotted");
            GUIContent errorIcon = EditorGUIUtility.IconContent("console.erroricon");
            GUIContent cancelledIcon = EditorGUIUtility.IconContent("CrossIcon");
            GUIContent playIcon = EditorGUIUtility.IconContent("d_Play@2x");
            GUIContent completeIcon = EditorGUIUtility.IconContent("d_GreenCheckmark@2x");

            label = new GUIContent(state.Actions[target].GetType().Name)
            {
                image = state.IsFinished || state.IsFailed ? cancelledIcon.image : inactiveIcon.image,
            };

            if (target == state.ExecutionIndex && !state.IsFinished && !state.IsFailed)
            {
                actionState = executingColor;
                label.image = playIcon.image;
            }

            if (target < state.ExecutionIndex || (target == state.ExecutionIndex && state.IsFinished))
            {
                actionState = completedColor;
                label.image = completeIcon.image;
            }

            if (target == state.ExecutionIndex && state.IsFailed)
            {
                actionState = failedColor;
                label.image = errorIcon.image;
            }
        }

        private void StoreInvokedExecutionState(ActionInvoker.ExecutionState state, ActionInvoker invoker)
        {
            if (!executionStates.ContainsKey(invoker))
            {
                executionStates.Add(invoker, new List<ActionInvoker.ExecutionState>());
            }

            executionStates[invoker].Add(state);
        }

        private Rect DrawHistoryDropdown(Rect position, ActionInvoker script)
        {
            Rect foldoutHistory = position;
            foldoutHistory.height = CalculateHeightWithSpacing(1);
            script.ShowHistory = EditorGUI.Foldout(foldoutHistory, script.ShowHistory, new GUIContent("History"));
            Rect historyRect = PushLine(foldoutHistory);

            if (!script.ShowHistory)
            {
                return historyRect;
            }

            foreach (IAction action in script.DebugActionHistory)
            {
                EditorGUI.LabelField(historyRect, new GUIContent(action.GetType().Name));
                historyRect = PushLine(historyRect);
            }

            position.y = historyRect.y + historyRect.height;
            return position;
        }
    }
}
#endif
