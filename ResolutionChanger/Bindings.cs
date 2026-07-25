namespace ResolutionChanger;

internal sealed class Bindings : Form
{
    private readonly BindingStore _store;
    private readonly HotkeyManager _hotkeys;
    private readonly DataGridView _grid = new();

    public Bindings(BindingStore store, HotkeyManager hotkeys)
    {
        _store = store;
        _hotkeys = hotkeys;
        InitializeUi();
        RefreshBindings();
    }

    private void InitializeUi()
    {
        Text = "Resolution bindings";
        Icon = ApplicationIcon.Shared;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(540, 300);
        Size = new Size(680, 400);
        _grid.Dock = DockStyle.Fill;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Display & Resolution",
                Name = "DisplayResolution",
                ReadOnly = true,
            }
        );
        _grid.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Hotkey",
                Name = "Hotkey",
                ReadOnly = true,
                FillWeight = 45,
            }
        );
        _grid.CellClick += EditCell;
        ContextMenuStrip menu = new();
        menu.Items.Add("Add binding", null, (_, _) => AddBinding());
        menu.Items.Add("Remove selected", null, (_, _) => RemoveSelected());
        _grid.ContextMenuStrip = menu;
        Controls.Add(_grid);
    }

    private void AddBinding()
    {
        using DisplayResolutionDialog dialog = new(null);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            SaveBinding(dialog.Binding!);
        }
    }

    private void EditCell(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        ResolutionBinding binding = (ResolutionBinding)_grid.Rows[e.RowIndex].Tag!;
        if (e.ColumnIndex == 0)
        {
            using DisplayResolutionDialog dialog = new(binding);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveBinding(dialog.Binding!);
            }
        }
        else
        {
            using HotkeyDialog dialog = new(binding, _hotkeys.FindBinding);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveBinding(dialog.Binding!);
            }
        }
    }

    private void SaveBinding(ResolutionBinding binding)
    {
        _store.Upsert(binding);
        _hotkeys.Register(_store.Bindings);
        RefreshBindings();
    }

    private void RemoveSelected()
    {
        if (_grid.CurrentRow?.Tag is not ResolutionBinding binding)
        {
            return;
        }

        _store.Remove(binding.Id);
        _hotkeys.Register(_store.Bindings);
        RefreshBindings();
    }

    private void RefreshBindings()
    {
        _grid.Rows.Clear();
        foreach (ResolutionBinding binding in _store.Bindings)
        {
            int row = _grid.Rows.Add($"{binding.DisplayName} - {binding.Width} × {binding.Height}", binding.HotkeyText);
            _grid.Rows[row].Tag = binding;
        }
    }
}
