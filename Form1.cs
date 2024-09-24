using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Windows.Forms;
using Truman_catalog;

namespace Truman_catalog
{

    public partial class Form1 : Form
    {
        private List<Product> filteredProducts; // Хранит текущий список отфильтрованных и отсортированных продуктов
        private int currentPage = 1;             // Текущая страница пагинации
        private int productsPerPage = 10;        // Количество продуктов на одной странице

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var context = new MyDbContext())
            {
                context.Database.EnsureCreated();

                // Добавление нескольких продуктов
                
                var products = context.Products.Include(p => p.Manufacturer).ToList();

                foreach (var product in products)
                {
                    // Создаем экземпляр ProductUserControl и передаем в него продукт
                    var productControl = new productUserControl(product);

                    // Добавляем ProductUserControl в FlowLayoutPanel
                    flowLayoutPanel1.Controls.Add(productControl);
                }
                ProdFiltBox.Items.Clear();
                ProdFiltBox.Items.Add("All"); // Добавляем вариант "All" для всех производителей
                ProdFiltBox.SelectedIndex = 0;
                foreach (var manufacturer in context.Manufacturers)
                {

                    ProdFiltBox.Items.Add($"{manufacturer.Name}");
                }
            }
            SortBox.Items.Add("Name");
            SortBox.Items.Add("Price ↑");
            SortBox.Items.Add("Price ↓");
            SortBox.Items.Add("Quantity ↑");
            SortBox.Items.Add("Quantity ↓");
            SortBox.Items.Add("Manufacturer ↑");
        }
        private void LoadProducts()
        {
            using (var context = new MyDbContext())
            {
                var query = context.Products.Include(p => p.Manufacturer).AsQueryable();

                // Фильтрация по производителю
                if (ProdFiltBox.SelectedIndex > 0) // Производитель выбран
                {
                    string selectedManufacturer = ProdFiltBox.SelectedItem.ToString();
                    query = query.Where(p => p.Manufacturer.Name == selectedManufacturer);
                }

                // Поиск по имени продукта
                string searchTerm = FindBox.Text.Trim();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => p.Name.Contains(searchTerm));
                }

                // Сортировка продуктов
                string sortField = SortBox.SelectedItem?.ToString();
                switch (sortField)
                {
                    case "Name ↑":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "Price ↑":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "Price ↓":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "Quantity ↑":
                        query = query.OrderBy(p => p.Quantity);
                        break;
                    case "Quantity ↓":
                        query = query.OrderByDescending(p => p.Quantity);
                        break;
                    case "Manufacturer ↑":
                        query = query.OrderBy(p => p.Manufacturer.Name);
                        break;
                    default:
                        // По умолчанию сортировка по имени
                        query = query.OrderBy(p => p.Name);
                        break;
                }

                // Пагинация
                int skipAmount = (currentPage - 1) * productsPerPage;
                query = query.Skip(skipAmount).Take(productsPerPage);

                filteredProducts = query.ToList();

                // Очистка FlowLayoutPanel перед добавлением новых элементов
                flowLayoutPanel1.Controls.Clear();

                foreach (var product in filteredProducts)
                {
                    var productControl = new productUserControl(product);
                    flowLayoutPanel1.Controls.Add(productControl);
                }
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ProdFiltBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void SortBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void FindBox_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void AddingButton_Click(object sender, EventArgs e)
        {
            AddingForm addingForm = new AddingForm();
            addingForm.ShowDialog();
            LoadProducts();
            using (var context = new MyDbContext())
            {
                ProdFiltBox.Items.Clear();
                ProdFiltBox.Items.Add("All"); // Добавляем вариант "All" для всех производителей
                ProdFiltBox.SelectedIndex = 0;

                foreach (var manufacturer in context.Manufacturers)
                {

                    ProdFiltBox.Items.Add($"{manufacturer.Name}");
                }
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadProducts(); // Загрузка предыдущей страницы
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            using (var context = new MyDbContext())
            {
                int maxPage = (int)Math.Ceiling((double)context.Products.Count() / productsPerPage);
                if (currentPage < maxPage)
                {
                    currentPage++;
                    LoadProducts(); // Загрузка следующей страницы
                }
            }
        }
    }
}
