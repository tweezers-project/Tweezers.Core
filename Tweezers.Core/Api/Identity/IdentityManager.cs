﻿using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Identity
{
    public static class IdentityManager
    {
        public static bool UsingIdentity { get; private set; }

        public static void RegisterIdentity()
        {
            var usersSchema = CreateUsersSchema(true);

            if (TweezersSchemaFactory.Find(usersSchema.CollectionName, true) == null)
                TweezersSchemaFactory.AddObject(usersSchema);
            else
            {
                TweezersSchemaFactory.DeleteObject(usersSchema.CollectionName);
                TweezersSchemaFactory.AddObject(usersSchema);
            }

            UsingIdentity = true;
        }

        public static TweezersObject CreateUsersSchema(bool withInternals = false)
        {
            TweezersObject usersSchema = new TweezersObject()
            {
                CollectionName = "users",
                Internal = true,
            };

            usersSchema.DisplayNames.SingularName = "User";
            usersSchema.DisplayNames.PluralName = "Users";
            usersSchema.Icon = "person";

            usersSchema.Fields.Add("username", new TweezersField()
            {
                Name = "username",
                DisplayName = "Username",
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                    UiTitle = true,
                    Min = 1,
                    Max = 50,
                    Required = true,
                    Regex = @"[A-Za-z\d]+"
                }
            });

            if (withInternals)
            {
                usersSchema.Fields.Add("passwordHash", new TweezersField()
                {
                    Name = "passwordHash",
                    DisplayName = "password",
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        Required = true,
                        UiIgnore = true,
                    }
                });

                usersSchema.Fields.Add("sessionId", new TweezersField()
                {
                    Name = "sessionId",
                    DisplayName = "Session ID",
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        UiIgnore = true,
                    }
                });

                usersSchema.Fields.Add("sessionExpiry", new TweezersField()
                {
                    Name = "sessionExpiry",
                    DisplayName = "SessionExpiry",
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        UiIgnore = true,
                    }
                });
            }

            return usersSchema;
        }
    }
}