using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.BLL;

internal class PersonService(TreeCacheDTO Cache)
{
    //Добавить сущность в древо; --- 
    //  Установить отношения (Указать, кто кому приходится родителем, ребенком или супругом);
    //  Создать новое древо (очищение предыдущего построенного дерева).
    private TreeCacheDTO _cache = Cache;

    public TreeCacheDTO Tree => _cache;

    public void AddPersonToTree(PersonDTO person)
    {
        _cache.Persons.Add(person.Id, person);
    }

}
