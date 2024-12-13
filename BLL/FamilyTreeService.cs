using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.DAL.Infrastructure;
using FamilyTreeBlazor.DAL.Entities;

namespace FamilyTreeBlazor.BLL;

internal class FamilyTreeService(TreeCacheDTO Cache)
{
    //Создать сущность “Человек”;
    //Добавить сущность в древо;
    //Установить отношения (Указать, кто кому приходится родителем, ребенком или супругом);
    //Вывести ближайших родственников (родителей и детей); ---
    //Показать получившееся древо;  ---
    //Вычислить возраст предка при рождении потомка; 
    //Создать новое древо (очищение предыдущего построенного дерева).
    private readonly IRepository<Person> _repository;
    private readonly TreeCacheDTO _cache = Cache; 

    public TreeCacheDTO Tree => _cache;

    public PersonListDTO GetImmediateRelatives(int PersonId)
    {
        if (!_cache.Persons.TryGetValue(PersonId, out var person))
        {
            return new();
        }

        PersonListDTO subTree = new();

        List<PersonDTO> children = person.Children;
        List<PersonDTO> parents = person.Parents;
            
        subTree.Persons.AddRange(children);
        subTree.Persons.AddRange(parents);

        return subTree;
    }

    public int GetPredecessorAgeOnBirthOfSuccessor(int PredeccessorId, int SuccessorId)
    {
        // TODO 
        return 0;
    }

    public void CreateNewTree()
    {
        
    }

}
