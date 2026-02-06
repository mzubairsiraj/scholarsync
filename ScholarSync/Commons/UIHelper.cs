using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScholarSync.Commons
{
    
    public static class UIHelper
    {
        // Layout Constants
        public const int LABEL_WIDTH = 100;
        public const int CONTROL_WIDTH = 300;
        public const int CONTROL_HEIGHT = 50;
        public const int LABEL_HEIGHT = 30;
        public const int VERTICAL_SPACING = 70;
        public const int HORIZONTAL_SPACING = 20;
        public const int SECTION_SPACING = 30;
        public const int COLUMN_SPACING = 50;

        // Create Section Header
        public static int CreateSectionHeader(Panel parentPanel, string title, int yPosition)
        {
            Label sectionLabel = new Label
            {
                Text = title,
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPosition),
                Size = new Size(parentPanel.Width - 80, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Panel underline = new Panel
            {
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(0, yPosition + 38),
                Size = new Size(parentPanel.Width - 80, 2)
            };

            parentPanel.Controls.Add(sectionLabel);
            parentPanel.Controls.Add(underline);

            return yPosition + 60;
        }

        // Create Form Row with TextBox
        public static void CreateFormRow(Panel parentPanel, string labelText, ref TextBox textBox, int xPosition, int yPosition, string placeholder)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xPosition, yPosition),
                Size = new Size(LABEL_WIDTH, LABEL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };

            textBox = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xPosition + LABEL_WIDTH + HORIZONTAL_SPACING, yPosition),
                Size = new Size(CONTROL_WIDTH, CONTROL_HEIGHT),
                ForeColor = Color.Gray,
                Text = placeholder,
                Tag = placeholder
            };

            TextBox localTextBox = textBox;

            textBox.Enter += (s, e) =>
            {
                if (localTextBox.Text == localTextBox.Tag.ToString())
                {
                    localTextBox.Text = "";
                    localTextBox.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(localTextBox.Text))
                {
                    localTextBox.Text = localTextBox.Tag.ToString();
                    localTextBox.ForeColor = Color.Gray;
                }
            };

            parentPanel.Controls.Add(label);
            parentPanel.Controls.Add(textBox);
        }

        // Create Password Row
        public static void CreatePasswordRow(Panel parentPanel, string labelText, ref TextBox textBox, int xPosition, int yPosition)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xPosition, yPosition),
                Size = new Size(LABEL_WIDTH, LABEL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };

            textBox = new TextBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xPosition + LABEL_WIDTH + HORIZONTAL_SPACING, yPosition),
                Size = new Size(CONTROL_WIDTH, CONTROL_HEIGHT),
                PasswordChar = '?',
                UseSystemPasswordChar = true
            };

            parentPanel.Controls.Add(label);
            parentPanel.Controls.Add(textBox);
        }

        // Create ComboBox Row
        public static void CreateComboBoxRow(Panel parentPanel, string labelText, ref ComboBox comboBox, int xPosition, int yPosition, string[] items)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xPosition, yPosition),
                Size = new Size(LABEL_WIDTH, LABEL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };

            comboBox = new ComboBox
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xPosition + LABEL_WIDTH + HORIZONTAL_SPACING, yPosition),
                Size = new Size(CONTROL_WIDTH, CONTROL_HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox.Items.AddRange(items);

            parentPanel.Controls.Add(label);
            parentPanel.Controls.Add(comboBox);
        }

        // Create DateTimePicker Row
        public static void CreateDatePickerRow(Panel parentPanel, string labelText, ref DateTimePicker dateTimePicker, int xPosition, int yPosition)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xPosition, yPosition),
                Size = new Size(LABEL_WIDTH, LABEL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dateTimePicker = new DateTimePicker
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(xPosition + LABEL_WIDTH + HORIZONTAL_SPACING, yPosition),
                Size = new Size(CONTROL_WIDTH, CONTROL_HEIGHT),
                Format = DateTimePickerFormat.Short
            };

            parentPanel.Controls.Add(label);
            parentPanel.Controls.Add(dateTimePicker);
        }

        // Create CheckBox Row
        public static void CreateCheckBoxRow(Panel parentPanel, string labelText, ref CheckBox checkBox, int xPosition, int yPosition)
        {
            checkBox = new CheckBox
            {
                Text = labelText,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSDarkBlueColor,
                Location = new Point(xPosition, yPosition),
                Size = new Size(LABEL_WIDTH + CONTROL_WIDTH + HORIZONTAL_SPACING, CONTROL_HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft
            };

            parentPanel.Controls.Add(checkBox);
        }

        // Create Standard Button
        public static Button CreateButton(string text, Color backColor, Point location, Size size)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                BackColor = backColor,
                Location = location,
                Size = size,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // Create Standard Button with Default Size
        public static Button CreateButton(string text, Color backColor, Point location)
        {
            return CreateButton(text, backColor, location, new Size(200, 50));
        }

        // Create Pagination Button
        public static Button CreatePaginationButton(string text, int xPosition)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = ConfigurationConstants.SSWhiteColor,
                BackColor = ConfigurationConstants.SSDarkNavyColor,
                Location = new Point(xPosition, 18),
                Size = new Size(50, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // Setup Placeholder Text Behavior for TextBox
        public static void SetupPlaceholderBehavior(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.Tag = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == textBox.Tag.ToString())
                {
                    textBox.Text = "";
                    textBox.ForeColor = ConfigurationConstants.SSDarkBlueColor;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = textBox.Tag.ToString();
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        // Update Button State (for pagination)
        public static void UpdateButtonState(Button btn)
        {
            if (!btn.Enabled)
            {
                btn.BackColor = Color.Gray;
                btn.ForeColor = ConfigurationConstants.SSWhiteColor;
            }
            else
            {
                btn.BackColor = ConfigurationConstants.SSDarkNavyColor;
                btn.ForeColor = ConfigurationConstants.SSWhiteColor;
            }
        }

        // Validate if TextBox has placeholder text
        public static bool IsPlaceholderText(TextBox textBox)
        {
            return textBox.ForeColor == Color.Gray || 
                   (textBox.Tag != null && textBox.Text == textBox.Tag.ToString());
        }

        // Clear TextBox to placeholder
        public static void ClearToPlaceholder(TextBox textBox)
        {
            if (textBox.Tag != null)
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.ForeColor = Color.Gray;
            }
            else
            {
                textBox.Clear();
            }
        }
    }
}
