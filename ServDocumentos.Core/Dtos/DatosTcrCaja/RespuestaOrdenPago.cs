using System.Collections.Generic;

namespace ServDocumentos.Core.Dtos.DatosTcrCaja
{
    public partial class RespuestaOrdenPago : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private List<DatosOrdenPago> ListaOrdenesPagoField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CodigoRespuestaField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MensajeField;

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
        public List<DatosOrdenPago> ListaDatosOrdenPago
        {
            get
            {
                return ListaOrdenesPagoField;
            }
            set
            {
                if ((object.ReferenceEquals(ListaOrdenesPagoField, value) != true))
                {
                    ListaOrdenesPagoField = value;
                    RaisePropertyChanged("ListaDatosOrdenPago");
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
