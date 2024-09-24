using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Truman_catalog
{
    public partial class productUserControl : UserControl
    {
        public productUserControl(Product product)
        {
            InitializeComponent();
            pictureBox1.Image = Image.FromFile(product.Path);
            NameLabel.Text = product.Name;
            DesLabel.Text = product.Description;
            ProdLabel.Text = product.Manufacturer.Name;
            PriceLabel.Text = product.Price.ToString();
            QuanLabel.Text = product.Quantity.ToString();
            if (product.Quantity > 0)
            {
                this.BackColor = Color.LightSkyBlue;
            }
            else
            {
                this.BackColor = Color.LightGray;
            }

        }

    }
}
