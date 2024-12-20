using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(300, 200);

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GeneralEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: (port) => 
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts; 
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private Node GeneralEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "Start",
            GUID = GUID.Generate().ToString(),
            dialogueText = "ENTRYPOINT",
            entryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.styleSheets.Add(Resources.Load<StyleSheet>("EntryNode"));

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            dialogueText = nodeName,
            GUID = GUID.Generate().ToString(),
            expanded = true
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var button = new Button(clickEvent: () => { AddChoicePort(dialogueNode); } );
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);

        dialogueNode.mainContainer.Add(textField);
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overridenPortName) ? $"Choice{outputPortCount + 1}" : overridenPortName;

        var textField = new TextField
        {
            name = "txt",
            value = choicePortName
        };


        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        var flexibleSpace = new VisualElement();
        flexibleSpace.style.flexGrow = 1;
        flexibleSpace.Add(textField);

        generatedPort.contentContainer.Add(flexibleSpace);

        var deleteButton = new Button(()=>RemovePort(dialogueNode, generatedPort));
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();

        //Debug.Log("DialogueNode.ports count: " + dialogueNode.outputContainer.Query(name: "connector").ToList().Count);
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => 
                        x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
