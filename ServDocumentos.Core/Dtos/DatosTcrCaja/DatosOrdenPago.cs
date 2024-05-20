using System;

namespace ServDocumentos.Core.Dtos.DatosTcrCaja
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "DatosOrdenPago", Namespace = "http://org.apache.synapse/xsd")]
    [System.SerializableAttribute()]
    public partial class DatosOrdenPago : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private int NoClienteField;

        private string BeneficiarioField;

        private string ConceptoField;

        private string ReferenciaField;

        private double MontoField;

        private DateTime FechaOperacionField;

        private string BancoField;

        private string NoConvenioField;

        private int DiasVigenciaField;

        private DateTime FechaVigenciaField;

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

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 1)]
        public int NumeroCliente
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
                    this.RaisePropertyChanged("NumeroCliente");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 2)]
        public string Nombre
        {
            get
            {
                return this.BeneficiarioField;
            }
            set
            {
                if ((object.ReferenceEquals(this.BeneficiarioField, value) != true))
                {
                    this.BeneficiarioField = value;
                    this.RaisePropertyChanged("Nombre");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 3)]
        public string Concepto
        {
            get
            {
                return this.ConceptoField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ConceptoField, value) != true))
                {
                    this.ConceptoField = value;
                    this.RaisePropertyChanged("Concepto");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 4)]
        public string Referencia
        {
            get
            {
                return this.ReferenciaField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ReferenciaField, value) != true))
                {
                    this.ReferenciaField = value;
                    this.RaisePropertyChanged("Referencia");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 5)]
        public double Monto
        {
            get
            {
                return this.MontoField;
            }
            set
            {
                if ((object.ReferenceEquals(this.MontoField, value) != true))
                {
                    this.MontoField = value;
                    this.RaisePropertyChanged("Monto");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 6)]
        public DateTime FechaOperacion
        {
            get
            {
                return this.FechaOperacionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FechaOperacionField, value) != true))
                {
                    this.FechaOperacionField = value;
                    this.RaisePropertyChanged("FechaOperacion");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 7)]
        public string Banco
        {
            get
            {
                return this.BancoField;
            }
            set
            {
                if ((object.ReferenceEquals(this.BancoField, value) != true))
                {
                    this.BancoField = value;
                    this.RaisePropertyChanged("Banco");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 8)]
        public string NumeroConvenio
        {
            get
            {
                return this.NoConvenioField;
            }
            set
            {
                if ((object.ReferenceEquals(this.NoConvenioField, value) != true))
                {
                    this.NoConvenioField = value;
                    this.RaisePropertyChanged("NumeroConvenio");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 9)]
        public int DiasVigencia
        {
            get
            {
                return this.DiasVigenciaField;
            }
            set
            {
                if ((this.DiasVigenciaField.Equals(value) != true))
                {
                    this.DiasVigenciaField = value;
                    this.RaisePropertyChanged("DiasVigencia");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Order = 10)]
        public DateTime FechaVigencia
        {
            get
            {
                return this.FechaVigenciaField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FechaVigenciaField, value) != true))
                {
                    this.FechaVigenciaField = value;
                    this.RaisePropertyChanged("FechaVigencia");
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
