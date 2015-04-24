# NZazu

[![Build status](https://ci.appveyor.com/api/projects/status/nj8cqgfnqd07csuc/branch/master?svg=true)](https://ci.appveyor.com/project/awesome-inc-build/nzazu/branch/master)

NZazu is a form templating engine which renders a form based on an abstract form definition.

The main view **NZazuView** connects factories, strategies and serializer to be flexible and extensible.

## FormDefinition

A **FormDefinition** defines a form with field types, descriptive texts and prompts. The definition is 
independent from the kind of data storage (e.g. object, dictionary) or rendering (e.g. wpf controls from 
[Xceeds Extended WPF Toolkit](http://wpftoolkit.codeplex.com/))


    FormDefinition = new FormDefinition
    {
        Fields = new[]
        {
            new FieldDefinition
            {
                Key = "settings", 
                Type = "label",
                Prompt = "Settings",
                Description = "You can manage your account here."
            },
            new FieldDefinition
            {
                Key = "name", 
                Type = "string",
                Prompt = "Name",
                Hint = "Enter name",
                Description = "Your account name. Only alpha-numeric ..."
            },
            new FieldDefinition
            {
                Key = "isAdmin", 
                Type = "bool",
                //Prompt = "Is Admin",
                Hint = "Is Admin",
                Description = "Check to grant administrator permissions"
            }
        }
    }

## INZazuFieldFactory

The field factory creates fields (controls) based on a template definition. By default, the following 
factories are implemented.

- [x] NZazuFieldFactory (main package)
- [ ] XceedFieldFactory (xceed package)


## INZazuLayoutStrategy

The layout strategy provides the mechanism to render the generated fields into the **NZazuView**

- [x] GridLayoutStrategy (main package)
- [ ] StackedLayoutStrategy (main package)


