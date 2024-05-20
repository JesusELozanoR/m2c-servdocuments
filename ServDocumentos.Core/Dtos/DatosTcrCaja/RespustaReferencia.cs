namespace ServDocumentos.Core.Dtos.DatosTcrCaja
{
    public partial class RespuestaReferencia : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Codigo128PaynetField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Codigo128WalMartField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CodigoRespuestaField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private DatosCreditoReferencia DatosCreditoField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MensajeField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NoReferenciaPaynetField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NoReferenciaWalMartField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return extensionDataField;
            }
            set
            {
                extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Codigo128Paynet
        {
            get
            {
                return Codigo128PaynetField;
            }
            set
            {
                if ((object.ReferenceEquals(Codigo128PaynetField, value) != true))
                {
                    Codigo128PaynetField = value;
                    RaisePropertyChanged("Codigo128Paynet");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Codigo128WalMart
        {
            get
            {
                return Codigo128WalMartField;
            }
            set
            {
                if ((object.ReferenceEquals(Codigo128WalMartField, value) != true))
                {
                    Codigo128WalMartField = value;
                    RaisePropertyChanged("Codigo128WalMart");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRespuesta
        {
            get
            {
                return CodigoRespuestaField;
            }
            set
            {
                if ((CodigoRespuestaField.Equals(value) != true))
                {
                    CodigoRespuestaField = value;
                    RaisePropertyChanged("CodigoRespuesta");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public DatosCreditoReferencia DatosCredito
        {
            get
            {
                return DatosCreditoField;
            }
            set
            {
                if ((object.ReferenceEquals(DatosCreditoField, value) != true))
                {
                    DatosCreditoField = value;
                    RaisePropertyChanged("DatosCredito");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mensaje
        {
            get
            {
                return MensajeField;
            }
            set
            {
                if ((object.ReferenceEquals(MensajeField, value) != true))
                {
                    MensajeField = value;
                    RaisePropertyChanged("Mensaje");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NoReferenciaPaynet
        {
            get
            {
                return NoReferenciaPaynetField;
            }
            set
            {
                if ((object.ReferenceEquals(NoReferenciaPaynetField, value) != true))
                {
                    NoReferenciaPaynetField = value;
                    RaisePropertyChanged("NoReferenciaPaynet");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NoReferenciaWalMart
        {
            get
            {
                return NoReferenciaWalMartField;
            }
            set
            {
                if ((object.ReferenceEquals(NoReferenciaWalMartField, value) != true))
                {
                    NoReferenciaWalMartField = value;
                    RaisePropertyChanged("NoReferenciaWalMart");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
