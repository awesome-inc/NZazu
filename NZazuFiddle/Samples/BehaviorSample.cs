using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu;
using NZazu.Contracts;
using NZazu.FieldBehavior;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BehaviorSample : SampleBase
    {
        public BehaviorSample()
            : base(40)
        {
            // register behavior
            BehaviorExtender.Register<OpenUrlOnStringEnterBehavior>("OpenUrlOnStringEnter");
            BehaviorExtender.Register<SetBorderBehavior>("SetBorder");

            Sample = new SampleViewModel
            {
                Name = "Behavior",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "headercomment",
                            Prompt = "",
                            Type = "label",
                            Description = "in the dialog below you can enter a text and open a url by hitting STRG+Return"
                        },
                        new FieldDefinition
                        {
                            Key = "comment",
                            Type = "string",
                            Prompt = "Comment",
                            Hint = "",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "describe your impression of the weather",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }
                            },
#pragma warning disable 618
                            Behaviors = new[] { new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }},
#pragma warning restore 618
                        },
                        new FieldDefinition
                        {
                            Key ="group",
                            Type = "group",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "nested.comment", Type ="string",
#pragma warning disable 618
                                    Behaviors = new[]{new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }}
#pragma warning restore 618
                                }
                            }
                        },
                        // Check multiple behaviours
                        new FieldDefinition
                        {
                            Key = "input1",
                            Type = "string",
                            Prompt = "Input 1",
                            Hint = "Input something ...\r\nIn this textbox you can test 2 behaviours ('OpenUrlOnStringEnter', 'SetBorder')\r\n--> set on 'Behaviors' collection prop",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "non sense",
                            Behaviors = new List<BehaviorDefinition>() {new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }, new BehaviorDefinition { Name = "SetBorder" }},
                        },
                        // Check single behaviour and multiple behaviours coexistence
                        new FieldDefinition
                        {
                            Key = "input2",
                            Type = "string",
                            Prompt = "Input 2",
                            Hint = "Input something ...\r\nIn this textbox you can test 2 behaviours ('OpenUrlOnStringEnter', 'SetBorder')\r\n--> set one on 'Behavior' prop and the other one on the 'Behaviors' collection prop",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "non sense",
                            Behaviors = new List<BehaviorDefinition>()
                            {
                                new BehaviorDefinition { Name = "SetBorder" },
                                new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }
                            },
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "comment", "type in a url like http://google.de and open it with Ctrl+Enter" },
                    { "nested.comment", "type in a url like http://google.de and open it with Ctrl+Enter" },
                })
            };
        }

        #region behavior and behavior test

        // ReSharper disable once ClassNeverInstantiated.Local
        private class OpenUrlOnStringEnterBehavior : INZazuWpfFieldBehavior
        {
            private Control _control;
            private KeyEventHandler _handler;
            private Regex _regex = new Regex("^(http|https)://");

            // ReSharper disable once UnusedMember.Local
            public string UrlPattern
            {
                get => _regex.ToString();
                set => _regex = new Regex(value);
            }

            public void AttachTo(INZazuWpfField field, INZazuWpfView view)
            {
                if (field == null) throw new ArgumentNullException(nameof(field));
                if (view == null) throw new ArgumentNullException(nameof(view), "this value should not be null and for testing we check it here");
                var valueControl = field.ValueControl;

                _control = valueControl;

                _handler = HandleKeyUp;
                _control.KeyUp += _handler;
            }

            public void Detach()
            {
                if (_control == null) return;

                _control.KeyUp -= _handler;
                _handler = null;
                _control = null;
            }

            private void HandleKeyUp(object sender, KeyEventArgs e)
            {
                if (!((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Return))) return;

                var link = GetLinkAtPosition(((TextBox)_control).Text, ((TextBox)_control).SelectionStart);
                if (link == null) return;

                Process.Start(link);
            }

            private string GetLinkAtPosition(string text, int position)
            {
                var spaceBefore = text.Substring(0, position).LastIndexOf(" ", StringComparison.Ordinal);
                if (spaceBefore < 0) spaceBefore = 0;
                else spaceBefore += 1;

                var spaceAfter = text.Substring(position).IndexOf(" ", StringComparison.Ordinal);
                if (spaceAfter < 0) spaceAfter = text.Length;
                else spaceAfter += position;

                var length = spaceAfter - spaceBefore;
                if (length <= 10) return null;
                var textUnderCursor = text.Substring(spaceBefore, length);
                return _regex.IsMatch(textUnderCursor) ? textUnderCursor : null;
            }

            [TestFixture]
            // ReSharper disable once InconsistentNaming
            private class OpenUrlOnStringEnterBehavior_Should
            {
                [Test]
                [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 6, null)]
                [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 3, null)]
                [TestCase("asdfklj asöfkdljsa ftp://google.de fdöakfjl saöljad fösadfa", 25, null)]
                [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 25, "http://google.de")]
                [TestCase("http://google.de asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 4, "http://google.de")]
                [TestCase("http://google.de", 0, "http://google.de")]
                [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa http://google.de", 56, "http://google.de")]
                public void Extract_Url(string text, int position, string expected)
                {
                    var sut = new OpenUrlOnStringEnterBehavior();
                    var url = sut.GetLinkAtPosition(text, position);
                    url.Should().Be(expected);
                }
            }

        }

        private class SetBorderBehavior : INZazuWpfFieldBehavior
        {
            private Control _control;

            public void AttachTo(INZazuWpfField field, INZazuWpfView view)
            {
                if (field == null) throw new ArgumentNullException(nameof(field));
                var valueControl = field.ValueControl;

                _control = valueControl;
                _control.BorderThickness = new Thickness(3);
                _control.BorderBrush = SystemColors.HotTrackBrush;
            }

            public void Detach()
            {
                if (_control == null) return;
                _control = null;
            }

            [TestFixture]
            // ReSharper disable once InconsistentNaming
            private class SetBorderBehavior_Should
            {
                [Test]
                [STAThread]
                [Apartment(ApartmentState.STA)]
                public void Set_Border()
                {
                    var sut = new SetBorderBehavior();
                    var field = Substitute.For<INZazuWpfField>();
                    field.ValueControl.Returns(Substitute.For<Control>());
                    sut.AttachTo(field, Substitute.For<INZazuWpfView>());

                    var control = sut._control;
                    control.BorderBrush.Should().Be(SystemColors.HotTrackBrush);
                    control.BorderThickness.Should().Be(new Thickness(3));
                }
            }
        }

        #endregion
    }
}