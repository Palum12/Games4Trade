export default {
  methods: {
    errorPopUp (self) {
      self.$swal({
        title: 'Ups coś poszło nie tak !',
        type: 'error'
      })
    },
    simpleSuccessPopUp (self) {
      self.$swal({
        title: 'Akcja zakończona sukcesem !',
        type: 'success'
      })
    },
    confirmationDialog (self) {
      return new Promise((resolve, reject) => {
        self.$swal({
          title: 'Czy jesteś pewny ?',
          text: 'Uwaga tej akcji nie można cofnąć !',
          type: 'warning',
          showCancelButton: true
        }).then((result) => {
          if (result.value) {
            resolve()
          } else {
            reject(result.value)
          }
        })
      })
    }
  }
}
