namespace ServDocumentos.Core.Dtos.DatosTcrCaja
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "DatosCreditoReferencia", Namespace = "http://org.apache.synapse/xsd")]
    [System.SerializableAttribute()]
    public partial class DatosCreditoReferencia : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private int NoClienteField;

        private string NombreField;

        private string ProductoField;

        private string NoCreditoField;

        private string NoCuentaAhorroField;

        private string NombreComercialField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public int NoCliente
        {
            get
            {
                return this.NoClienteField;
            }
            set
            {
                if ((this.NoClienteField.Equals(value) != true))
                {
                    this.NoClienteField = value;
                    this.RaisePropertyChanged("NoCliente");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string Nombre
        {
            get
            {
                return this.NombreField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NombreField, value) != true))
                {
                    this.NombreField = value;
                    this.RaisePropertyChanged("Nombre");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public string Producto
        {
            get
            {
                return this.ProductoField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ProductoField, value) != true))
                {
                    this.ProductoField = value;
                    this.RaisePropertyChanged("Producto");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 3)]
        public string NoCredito
        {
            get
            {
                return this.NoCreditoField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NoCreditoField, value) != true))
                {
                    this.NoCreditoField = value;
                    this.RaisePropertyChanged("NoCredito");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 4)]
        public string NoCuentaAhorro
        {
            get
            {
                return this.NoCuentaAhorroField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NoCuentaAhorroField, value) != true))
                {
                    this.NoCuentaAhorroField = value;
                    this.RaisePropertyChanged("NoCuentaAhorro");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 5)]
        public string NombreComercial
        {
            get
            {
                return this.NombreComercialField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NombreComercialField, value) != true))
                {
                    this.NombreComercialField = value;
                    this.RaisePropertyChanged("NombreComercial");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
