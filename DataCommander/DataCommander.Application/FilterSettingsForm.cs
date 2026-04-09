using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;

namespace DataCommander.Application
{
    public partial class FilterSettingsForm : Form
    {
        private FilterCriterion[]? _filterCriteria;

        public FilterSettingsForm(ColorTheme? colorTheme, IReadOnlyCollection<string> filterableProperties, IReadOnlyCollection<FilterCriterion> filterCriteria)
        {
            InitializeComponent();
            dataGridView.Font = new Font("Microsoft Sans Serif", 8);

            colorTheme!.Apply(this);
            colorTheme!.Apply(dataGridView);

            foreach (var filterableProperty in filterableProperties)
            {
                var filterCriterion = filterCriteria.FirstOrDefault(c => c.Property == filterableProperty);
                var value = filterCriterion?.Value;
                dataGridView.Rows.Add(filterableProperty, value);
            }
        }

        public IReadOnlyCollection<FilterCriterion>? FilterCriteria => _filterCriteria;

        private void okButton_Click(object sender, EventArgs e)
        {
            var filterCriteria = new List<FilterCriterion>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var property = (string)row.Cells[0].Value!;
                var value = row.Cells[1].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    filterCriteria.Add(new FilterCriterion(property, value));
            }
            _filterCriteria = filterCriteria.ToArray();
        }
    }
}
