using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Processors;
using FamilyTreeBlazor.presentation.State.EditState;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Substates;
using FamilyTreeBlazor.presentation.State.Interfaces;
using Microsoft.JSInterop;

namespace FamilyTreeBlazor.presentation.Services.Commands;

public interface ICommand
{
    void Execute(IAppState context, IToolState state);
}

public interface IGeneralCommandMarker { }
public interface IEditMarker { }
public interface IViewMarker { }
public interface IAncestorAgeMarker { }
public interface ICommonAncestorsMarker { }

public class InitCommand : ICommand, IGeneralCommandMarker
{
    public void Execute(IAppState context, IToolState state)
    {
        // Execution logic for Init command
    }
}

public class CancelCommand : ICommand, IGeneralCommandMarker
{
    public void Execute(IAppState context, IToolState state)
    {
        state.Cancel();
    }
}

public class HandleIdCommand(int id) : ICommand, IGeneralCommandMarker
{
    public int Id { get; private set; } = id;

    public void Execute(IAppState context, IToolState state)
    {
        state.HandleId(Id);
    }
}

public class CreatePersonCommand : ICommand, IEditMarker
{
    public void Execute(IAppState context, IToolState state)
    {
        if (context is EditToolState editState)
        {
            editState.ChangeState(EditStateEnum.CreatePerson);
        }
    }
}

public class CreateInitialPersonCommand : ICommand, IEditMarker
{
    public void Execute(IAppState context, IToolState state)
    {
        if (context is EditToolState editState)
        {
            editState.ChangeState(EditStateEnum.CreateInitialPerson);
        }
    }
}

public class SetRelationTypeCommand(Relation relation) : ICommand, IEditMarker
{
    public Relation Relation { get; private set; } = relation;

    public void Execute(IAppState context, IToolState state)
    {
        if (context is EditToolState editState)
        {
            editState.SetRelationState(Relation);
        }
    }
}
public class AddPersonCommand(Person person, AddPersonProcessor processor) : ICommand, IEditMarker
{
    public Person Person { get; private set; } = person;

    public void Execute(IAppState context, IToolState state)
    {
        int EditId;
        Relation relation;
        if (context is EditToolState editTool)
        {
            EditId = editTool.EditId;
            relation = editTool.CurrentRelation;
        }
        else
        {
            Console.WriteLine($"AddPersonCommand cannot be executed in the current state {context.GetType().Name}");
            return;
        }

        if (state is CreateInitialPersonState)
        {
            processor.AddInitialPerson(Person);
            context.Fire(new CancelCommand());
        }
        else if (state is CreatePersonState)
        {
            processor.AddPersonWithRelation(EditId, relation, Person);
        }
    }
}

public class CreateRelationCommand(int targetId, CreateRelationProcessor processor) : ICommand, IEditMarker
{
    public int TargetId { get; private set; } = targetId;
    public CreateRelationProcessor Processor { get; private set; } = processor;

    public void Execute(IAppState context, IToolState state)
    {
        if (context is EditToolState editTool)
        {
            var relation = editTool.CurrentRelation;
            var editId = editTool.EditId;

            Processor.CreateRelation(editId, TargetId, relation);
            context.Fire(new CancelCommand());
        }
        else
        {
            Console.WriteLine($"CreateRelationCommand cannot be executed in the current state {context.GetType().Name}");
        }
    }
}

public class SetEditStateJSCommand(IJSRuntime runtime) : ICommand, IGeneralCommandMarker
{
    async public void Execute(IAppState context, IToolState state)
    {
        bool setTrue = false;
        if (context is EditToolState && state is ChoosePersonState) setTrue = true; 

        await runtime.InvokeVoidAsync("setEditMode", setTrue);
    }
}
