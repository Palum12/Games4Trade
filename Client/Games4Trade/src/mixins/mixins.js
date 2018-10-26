export default {
  methods: {
    errorPopUp (self) {
      self.$swal({
        title: 'Ups coś poszło nie tak !',
        type: 'error'
      })
    },
    customErrorPopUp (self, message) {
      self.$swal({
        title: 'Ups coś poszło nie tak !',
        text: message,
        type: 'error'
      })
    },
    simpleSuccessPopUp (self) {
      self.$swal({
        title: 'Akcja zakończona sukcesem !',
        type: 'success'
      })
    },
    customSuccessPopUp (self, message) {
      self.$swal({
        title: 'Akcja zakończona sukcesem !',
        text: message,
        type: 'success'
      })
    },
    confirmationDialog (self) {
      return new Promise((resolve, reject) => {
        self.$swal({
          title: 'Czy jesteś pewny ?',
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
    },
    confirmationPernamentDialog (self) {
      return new Promise((resolve, reject) => {
        self.$swal({
          title: 'Czy jesteś pewny ? Tej akcji nie można cofnąć !',
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
    },
    confirmationLeaveDialog (self) {
      return new Promise((resolve, reject) => {
        self.$swal({
          title: 'Czy jesteś pewny ?',
          text: 'Nie zapisane zmiany zostaną usunięte!',
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
